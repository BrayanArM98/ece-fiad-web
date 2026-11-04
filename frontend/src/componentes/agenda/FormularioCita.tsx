import { format, isSameDay, isToday } from 'date-fns'
import { es } from 'date-fns/locale'
import { CalendarDays } from 'lucide-react'
import { useId, useState, type FormEvent } from 'react'
import { toast } from 'sonner'
import { Button } from '@/components/ui/button'
import { Calendar } from '@/components/ui/calendar'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { aFecha, aFechaApi, aFechaHoraApi, formatearFechaLarga, formatearHora } from '@/lib/formato'
import type { Cita, DatosCita } from '@/lib/tipos'
import { useAgendarCita, useCitas, useDoctores, useEditarCita, usePacientesActivos } from './consultas'
import { HORARIOS, hoy, textoHora, minutosDelDia, unirFechaHora } from './fechas'
import { SelectorPaciente } from './SelectorPaciente'
import { AvisoError, Campo, PieFormulario } from '@/componentes/Formulario'

export interface ValoresIniciales {
  fecha?: Date
  horario?: string
  idDoctor?: number
  idPaciente?: number
}

/**
 * Diálogo para agendar o editar una cita (specs/frontend-pantallas.md, "Agenda").
 * Valida lo mismo que la API (CrearCitaValidator) antes de enviar, y marca como ocupados
 * los horarios en que el doctor ya tiene cita ese día.
 */
export function FormularioCita({
  abierto,
  alCerrar,
  cita,
  iniciales,
  alGuardar,
}: {
  abierto: boolean
  alCerrar: () => void
  /** La cita a editar; sin ella el diálogo agenda una nueva. */
  cita?: Cita | null
  iniciales?: ValoresIniciales
  alGuardar?: (cita: Cita) => void
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="gap-0 p-0 sm:max-w-lg">
        {abierto ? (
          <CuerpoFormulario cita={cita ?? null} iniciales={iniciales} alCerrar={alCerrar} alGuardar={alGuardar} />
        ) : null}
      </DialogContent>
    </Dialog>
  )
}

interface Errores {
  paciente?: string
  doctor?: string
  fecha?: string
  horario?: string
  motivo?: string
}

function CuerpoFormulario({
  cita,
  iniciales,
  alCerrar,
  alGuardar,
}: {
  cita: Cita | null
  iniciales?: ValoresIniciales
  alCerrar: () => void
  alGuardar?: (cita: Cita) => void
}) {
  const editando = cita !== null
  const fechaOriginal = cita ? aFecha(cita.fechaHora) : null
  const horarioOriginal = cita ? textoHora(minutosDelDia(cita.fechaHora)) : null

  const [idPaciente, setIdPaciente] = useState<number | null>(cita?.idPaciente ?? iniciales?.idPaciente ?? null)
  const [idDoctor, setIdDoctor] = useState<number | null>(cita?.idDoctor ?? iniciales?.idDoctor ?? null)
  const [fecha, setFecha] = useState<Date | undefined>(
    fechaOriginal
      ? new Date(fechaOriginal.getFullYear(), fechaOriginal.getMonth(), fechaOriginal.getDate())
      : (iniciales?.fecha ?? hoy()),
  )
  const [horario, setHorario] = useState(horarioOriginal ?? iniciales?.horario ?? '')
  const [motivo, setMotivo] = useState(cita?.motivo ?? '')
  const [notas, setNotas] = useState(cita?.notas ?? '')
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const [calendarioAbierto, setCalendarioAbierto] = useState(false)

  const ids = { paciente: useId(), doctor: useId(), fecha: useId(), horario: useId(), motivo: useId(), notas: useId() }

  const pacientes = usePacientesActivos()
  const doctores = useDoctores()
  const agendar = useAgendarCita()
  const editar = useEditarCita()
  const enviando = agendar.isPending || editar.isPending

  // Citas del doctor ese día, para marcar los horarios ocupados
  const diaApi = fecha ? aFechaApi(fecha) : ''
  const citasDelDia = useCitas({ desde: diaApi, hasta: diaApi, idDoctor: idDoctor ?? undefined }, !!fecha && !!idDoctor)
  const ocupados = new Set(
    (citasDelDia.data ?? [])
      .filter((c) => c.estado !== 'Cancelada' && c.id !== cita?.id)
      .map((c) => textoHora(minutosDelDia(c.fechaHora))),
  )

  const esOriginal = (h: string) =>
    !!fecha && !!fechaOriginal && isSameDay(fecha, fechaOriginal) && h === horarioOriginal
  const yaPaso = (h: string) => !!fecha && unirFechaHora(fecha, h) <= new Date() && !esOriginal(h)
  const opcionesHorario =
    horarioOriginal && !HORARIOS.includes(horarioOriginal) && esOriginal(horarioOriginal)
      ? [...HORARIOS, horarioOriginal].sort()
      : HORARIOS

  // Si el paciente o el doctor de la cita ya no están activos, se agregan para poder mostrarlos
  const listaPacientes = pacientes.data ?? []
  const listaDoctores = doctores.data ?? []
  const pacientesConActual =
    cita && pacientes.data && !listaPacientes.some((p) => p.id === cita.idPaciente)
      ? [
          ...listaPacientes,
          {
            id: cita.idPaciente,
            nombres: cita.nombrePaciente,
            apellidos: '',
            numeroDocumento: '',
          } as (typeof listaPacientes)[number],
        ]
      : listaPacientes

  const errores: Errores = {}
  if (!idPaciente) errores.paciente = 'Elige un paciente.'
  if (!idDoctor) errores.doctor = 'Elige un doctor.'
  if (!fecha) errores.fecha = 'Elige la fecha.'
  if (!horario) errores.horario = 'Elige la hora.'
  else if (yaPaso(horario)) errores.horario = 'La hora ya pasó; elige un horario futuro.'
  else if (ocupados.has(horario)) errores.horario = 'El doctor ya tiene una cita a esa hora.'
  if (!motivo.trim()) errores.motivo = 'Escribe el motivo de la cita.'
  const hayErrores = Object.keys(errores).length > 0
  const error = (campo: keyof Errores) => (mostrarErrores ? errores[campo] : undefined)

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (hayErrores || !fecha || !idPaciente || !idDoctor) return

    const fechaHora = unirFechaHora(fecha, horario)
    const datos: DatosCita = {
      idPaciente,
      idDoctor,
      fechaHora: aFechaHoraApi(fechaHora),
      motivo: motivo.trim(),
      notas: notas.trim(),
      estado: cita?.estado ?? 'Pendiente',
    }
    const opciones = {
      onSuccess: (guardada: Cita) => {
        toast.success(
          editando
            ? 'Cambios guardados.'
            : `Cita agendada para el ${formatearFechaLarga(fechaHora)} a las ${formatearHora(fechaHora)}.`,
        )
        alGuardar?.(guardada)
        alCerrar()
      },
      onError: (e: unknown) => setErrorApi(e),
    }

    if (cita) editar.mutate({ id: cita.id, datos }, opciones)
    else agendar.mutate(datos, opciones)
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {editando ? 'Editar cita' : 'Agendar cita'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          {editando
            ? 'Cambia el paciente, el doctor, el horario o el motivo.'
            : 'Los horarios ocupados del doctor aparecen deshabilitados.'}
        </DialogDescription>
      </DialogHeader>

      <div className="grid gap-5 p-6">
        {errorApi ? <AvisoError error={errorApi} titulo="Revisa los datos de la cita:" /> : null}

        {pacientes.isError || doctores.isError ? (
          <AvisoError error={pacientes.error ?? doctores.error} />
        ) : null}

        <Campo etiqueta="Paciente" htmlFor={ids.paciente} error={error('paciente')}>
          <SelectorPaciente
            id={ids.paciente}
            pacientes={pacientesConActual}
            cargando={pacientes.isPending}
            valor={idPaciente}
            alCambiar={setIdPaciente}
            invalido={!!error('paciente')}
            descripcionId={error('paciente') ? `${ids.paciente}-error` : undefined}
          />
        </Campo>

        <Campo etiqueta="Doctor" htmlFor={ids.doctor} error={error('doctor')}>
          <Select
            value={idDoctor ? String(idDoctor) : ''}
            onValueChange={(v) => setIdDoctor(Number(v))}
            disabled={doctores.isPending}
          >
            <SelectTrigger
              id={ids.doctor}
              className="h-9 w-full"
              aria-invalid={!!error('doctor') || undefined}
              aria-describedby={error('doctor') ? `${ids.doctor}-error` : undefined}
            >
              <SelectValue placeholder={doctores.isPending ? 'Cargando doctores…' : 'Elige un doctor'} />
            </SelectTrigger>
            <SelectContent>
              {cita && doctores.data && !listaDoctores.some((d) => d.id === cita.idDoctor) ? (
                <SelectItem value={String(cita.idDoctor)}>{cita.nombreDoctor}</SelectItem>
              ) : null}
              {listaDoctores.map((d) => (
                <SelectItem key={d.id} value={String(d.id)}>
                  {d.nombreCompleto}
                  <span className="text-pizarra">· {d.nombreEspecialidad}</span>
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Campo>

        <div className="grid gap-5 sm:grid-cols-[1fr_10rem]">
          <Campo etiqueta="Fecha" htmlFor={ids.fecha} error={error('fecha')}>
            <Popover open={calendarioAbierto} onOpenChange={setCalendarioAbierto}>
              <PopoverTrigger asChild>
                <Button
                  id={ids.fecha}
                  type="button"
                  variant="outline"
                  className="h-9 w-full justify-start px-3 font-normal"
                  aria-invalid={!!error('fecha') || undefined}
                >
                  <CalendarDays className="text-pizarra" aria-hidden="true" />
                  {fecha ? format(fecha, "EEE d 'de' MMMM yyyy", { locale: es }) : 'Elige la fecha'}
                </Button>
              </PopoverTrigger>
              <PopoverContent className="w-auto p-0" align="start">
                <Calendar
                  mode="single"
                  locale={es}
                  weekStartsOn={1}
                  selected={fecha}
                  defaultMonth={fecha}
                  disabled={{ before: fechaOriginal && fechaOriginal < hoy() ? fechaOriginal : hoy() }}
                  onSelect={(dia) => {
                    setFecha(dia)
                    setCalendarioAbierto(false)
                  }}
                />
              </PopoverContent>
            </Popover>
          </Campo>

          <Campo etiqueta="Hora" htmlFor={ids.horario} error={error('horario')}>
            <Select value={horario} onValueChange={setHorario}>
              <SelectTrigger
                id={ids.horario}
                className="h-9 w-full"
                aria-invalid={!!error('horario') || undefined}
                aria-describedby={error('horario') ? `${ids.horario}-error` : undefined}
              >
                <SelectValue placeholder="--:--" />
              </SelectTrigger>
              <SelectContent className="max-h-72">
                {opcionesHorario.map((h) => {
                  const ocupado = ocupados.has(h)
                  const pasado = yaPaso(h)
                  return (
                    <SelectItem key={h} value={h} disabled={ocupado || pasado} className="cifras">
                      {h}
                      {ocupado ? <span className="text-xs text-pizarra">ocupado</span> : null}
                    </SelectItem>
                  )
                })}
              </SelectContent>
            </Select>
          </Campo>
        </div>
        {fecha && isToday(fecha) && !editando ? (
          <p className="-mt-3 text-xs text-pizarra">Para hoy, los horarios que ya pasaron aparecen deshabilitados.</p>
        ) : null}

        <Campo etiqueta="Motivo" htmlFor={ids.motivo} error={error('motivo')}>
          <Input
            id={ids.motivo}
            value={motivo}
            onChange={(e) => setMotivo(e.target.value)}
            maxLength={500}
            placeholder="Por ejemplo: revisión de presión arterial"
            aria-invalid={!!error('motivo') || undefined}
            aria-describedby={error('motivo') ? `${ids.motivo}-error` : undefined}
          />
        </Campo>

        <Campo etiqueta="Notas" opcional htmlFor={ids.notas}>
          <Textarea
            id={ids.notas}
            value={notas}
            onChange={(e) => setNotas(e.target.value)}
            maxLength={1000}
            rows={3}
            placeholder="Indicaciones para el paciente o para el doctor"
          />
        </Campo>
      </div>

      <PieFormulario
        enviando={enviando}
        textoEnviar={editando ? 'Guardar cambios' : 'Agendar cita'}
        alCancelar={alCerrar}
      />
    </form>
  )
}
