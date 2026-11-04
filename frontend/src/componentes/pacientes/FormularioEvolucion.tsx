import { useId, useState, type FormEvent } from 'react'
import { toast } from 'sonner'
import { useDoctores } from '@/componentes/agenda/consultas'
import { AvisoError, Campo, conError, PieFormulario } from '@/componentes/Formulario'
import { Dialog, DialogContent, DialogDescription, DialogHeader, DialogTitle } from '@/components/ui/dialog'
import { Input } from '@/components/ui/input'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Textarea } from '@/components/ui/textarea'
import { aFechaHoraApi } from '@/lib/formato'
import type { Evolucion } from '@/lib/tipos'
import { useSesion } from '@/sesion/SesionContext'
import { useGuardarEvolucion } from './consultas'

/**
 * Registrar o editar una evolución (nota de consulta). Un médico siempre la registra a su nombre;
 * el administrador elige el doctor.
 */
export function FormularioEvolucion({
  abierto,
  alCerrar,
  idPaciente,
  idHistoria,
  evolucion,
}: {
  abierto: boolean
  alCerrar: () => void
  idPaciente: number
  idHistoria: number
  evolucion: Evolucion | null
}) {
  return (
    <Dialog open={abierto} onOpenChange={(siguiente) => !siguiente && alCerrar()}>
      <DialogContent className="max-h-[calc(100dvh-2rem)] gap-0 overflow-y-auto p-0 sm:max-w-lg">
        {abierto ? (
          <Cuerpo idPaciente={idPaciente} idHistoria={idHistoria} evolucion={evolucion} alCerrar={alCerrar} />
        ) : null}
      </DialogContent>
    </Dialog>
  )
}

function Cuerpo({
  idPaciente,
  idHistoria,
  evolucion,
  alCerrar,
}: {
  idPaciente: number
  idHistoria: number
  evolucion: Evolucion | null
  alCerrar: () => void
}) {
  const { usuario } = useSesion()
  const esMedico = usuario?.rol === 'Medico'
  const doctores = useDoctores()
  const [idDoctor, setIdDoctor] = useState<number | null>(evolucion?.idDoctor ?? (esMedico ? usuario?.idDoctor ?? null : null))
  const [diagnostico, setDiagnostico] = useState(evolucion?.diagnostico ?? '')
  const [tratamiento, setTratamiento] = useState(evolucion?.tratamiento ?? '')
  const [notas, setNotas] = useState(evolucion?.notas ?? '')
  const [mostrarErrores, setMostrarErrores] = useState(false)
  const [errorApi, setErrorApi] = useState<unknown>(null)
  const guardar = useGuardarEvolucion(idPaciente)
  const base = useId()
  const id = (campo: string) => `${base}-${campo}`

  const errores: Record<string, string | undefined> = {
    doctor: !esMedico && !idDoctor ? 'Elige el doctor que atendió.' : undefined,
    diagnostico: !diagnostico.trim() ? 'Escribe el diagnóstico.' : undefined,
    tratamiento: !tratamiento.trim() ? 'Escribe el tratamiento indicado.' : undefined,
  }
  const error = (campo: string) => (mostrarErrores ? errores[campo] : undefined)

  const enviar = (evento: FormEvent) => {
    evento.preventDefault()
    setMostrarErrores(true)
    setErrorApi(null)
    if (Object.values(errores).some(Boolean)) return
    guardar.mutate(
      {
        idHistoria,
        evolucion,
        fecha: aFechaHoraApi(new Date()),
        datos: {
          idDoctor: idDoctor ?? 0,
          diagnostico: diagnostico.trim(),
          tratamiento: tratamiento.trim(),
          notas: notas.trim(),
        },
      },
      {
        onSuccess: () => {
          toast.success(evolucion ? 'Evolución actualizada.' : 'Evolución registrada.')
          alCerrar()
        },
        onError: setErrorApi,
      },
    )
  }

  return (
    <form onSubmit={enviar} noValidate className="grid">
      <DialogHeader className="border-b border-linea p-6">
        <DialogTitle className="font-heading text-lg font-semibold">
          {evolucion ? 'Editar evolución' : 'Registrar evolución'}
        </DialogTitle>
        <DialogDescription className="text-pizarra">
          {esMedico ? 'La evolución queda registrada a tu nombre.' : 'Nota de la consulta: diagnóstico, tratamiento y observaciones.'}
        </DialogDescription>
      </DialogHeader>
      <div className="grid gap-5 p-6">
        {errorApi ? <AvisoError error={errorApi} titulo="Revisa los datos de la evolución:" /> : null}
        {!esMedico ? (
          <Campo etiqueta="Doctor" htmlFor={id('doctor')} error={error('doctor')}>
            <Select value={idDoctor ? String(idDoctor) : ''} onValueChange={(v) => setIdDoctor(Number(v))}>
              <SelectTrigger id={id('doctor')} className="h-9 w-full" {...conError(id('doctor'), error('doctor'))}>
                <SelectValue placeholder={doctores.isPending ? 'Cargando doctores…' : 'Elige un doctor'} />
              </SelectTrigger>
              <SelectContent>
                {(doctores.data ?? []).map((d) => (
                  <SelectItem key={d.id} value={String(d.id)}>
                    {d.nombreCompleto}
                    <span className="text-pizarra">· {d.nombreEspecialidad}</span>
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Campo>
        ) : null}
        <Campo etiqueta="Diagnóstico" htmlFor={id('diagnostico')} error={error('diagnostico')}>
          <Input
            id={id('diagnostico')}
            value={diagnostico}
            maxLength={500}
            placeholder="Por ejemplo: infección respiratoria alta"
            onChange={(e) => setDiagnostico(e.target.value)}
            {...conError(id('diagnostico'), error('diagnostico'))}
          />
        </Campo>
        <Campo etiqueta="Tratamiento" htmlFor={id('tratamiento')} error={error('tratamiento')}>
          <Textarea
            id={id('tratamiento')}
            value={tratamiento}
            rows={3}
            maxLength={500}
            placeholder="Medicamentos, dosis y duración, reposo, estudios"
            onChange={(e) => setTratamiento(e.target.value)}
            {...conError(id('tratamiento'), error('tratamiento'))}
          />
        </Campo>
        <Campo etiqueta="Notas" opcional htmlFor={id('notas')}>
          <Textarea
            id={id('notas')}
            value={notas}
            rows={3}
            maxLength={1000}
            placeholder="Signos vitales, evolución, indicaciones de seguimiento"
            onChange={(e) => setNotas(e.target.value)}
          />
        </Campo>
      </div>
      <PieFormulario
        enviando={guardar.isPending}
        textoEnviar={evolucion ? 'Guardar cambios' : 'Registrar evolución'}
        alCancelar={alCerrar}
      />
    </form>
  )
}
