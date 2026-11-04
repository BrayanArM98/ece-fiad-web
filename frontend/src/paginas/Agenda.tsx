import { addDays, addWeeks, isSameDay } from 'date-fns'
import { es } from 'date-fns/locale'
import { CalendarDays, ChevronLeft, ChevronRight, Plus } from 'lucide-react'
import { useState } from 'react'
import { useSearchParams } from 'react-router'
import { DetalleCita } from '@/componentes/agenda/DetalleCita'
import { FormularioCita, type ValoresIniciales } from '@/componentes/agenda/FormularioCita'
import { useCitas, useDoctores } from '@/componentes/agenda/consultas'
import { diasDeLaSemana, hoy, leerFecha, tituloRango, type Vista } from '@/componentes/agenda/fechas'
import { VistaDia } from '@/componentes/agenda/VistaDia'
import { VistaSemana } from '@/componentes/agenda/VistaSemana'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista } from '@/componentes/EstadosVista'
import { Button } from '@/components/ui/button'
import { Calendar } from '@/components/ui/calendar'
import { Popover, PopoverContent, PopoverTrigger } from '@/components/ui/popover'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Skeleton } from '@/components/ui/skeleton'
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { ESTADOS_CITA } from '@/lib/estados'
import { aFecha, aFechaApi } from '@/lib/formato'
import type { Cita, EstadoCita } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { useSesion } from '@/sesion/SesionContext'

const TODOS = 'todos'

/**
 * Agenda (specs/frontend-pantallas.md, "Agenda"): vista de día con regla de horas y vista de semana.
 * La fecha, la vista y el doctor viven en la URL (?fecha=2026-11-04&vista=semana&doctor=2),
 * así que recargar o compartir el enlace conserva lo que se está viendo.
 */
export default function PaginaAgenda() {
  const { usuario } = useSesion()
  const esMedico = usuario?.rol === 'Medico'
  const puedeAgendar = usuario?.rol === 'Administrador' || usuario?.rol === 'Recepcionista'

  const [parametros, setParametros] = useSearchParams()
  const fecha = leerFecha(parametros.get('fecha'))
  const vista: Vista = parametros.get('vista') === 'semana' ? 'semana' : 'dia'
  const idDoctor = esMedico ? undefined : Number(parametros.get('doctor')) || undefined

  const actualizar = (cambios: { fecha?: Date; vista?: Vista; doctor?: number | null }) => {
    const siguientes = new URLSearchParams(parametros)
    if (cambios.fecha) {
      if (isSameDay(cambios.fecha, hoy())) siguientes.delete('fecha')
      else siguientes.set('fecha', aFechaApi(cambios.fecha))
    }
    if (cambios.vista) {
      if (cambios.vista === 'dia') siguientes.delete('vista')
      else siguientes.set('vista', cambios.vista)
    }
    if (cambios.doctor !== undefined) {
      if (cambios.doctor === null) siguientes.delete('doctor')
      else siguientes.set('doctor', String(cambios.doctor))
    }
    setParametros(siguientes, { replace: true })
  }

  const dias = diasDeLaSemana(fecha)
  const rango =
    vista === 'dia'
      ? { desde: aFechaApi(fecha), hasta: aFechaApi(fecha) }
      : { desde: aFechaApi(dias[0]), hasta: aFechaApi(dias[6]) }
  const citas = useCitas({ ...rango, idDoctor })
  const doctores = useDoctores()

  // Cita abierta en el panel lateral; se toma de la lista para ver los cambios al instante
  const [idAbierta, setIdAbierta] = useState<number | null>(null)
  const abierta = citas.data?.find((c) => c.id === idAbierta) ?? null

  const [formulario, setFormulario] = useState<{ cita: Cita | null; iniciales?: ValoresIniciales } | null>(null)

  const mover = (direccion: 1 | -1) =>
    actualizar({ fecha: vista === 'dia' ? addDays(fecha, direccion) : addWeeks(fecha, direccion) })

  const unidad = vista === 'dia' ? 'Día' : 'Semana'

  return (
    <div className="grid gap-6">
      <EncabezadoPagina
        titulo="Agenda"
        descripcion={esMedico ? 'Tus citas con los pacientes' : 'Citas de la clínica por día y por semana'}
        accion={
          puedeAgendar ? (
            <Button onClick={() => setFormulario({ cita: null, iniciales: { fecha, idDoctor } })}>
              <Plus aria-hidden="true" />
              Agendar cita
            </Button>
          ) : null
        }
      />

      {/* Barra de herramientas */}
      <div className="flex flex-wrap items-center gap-x-4 gap-y-3">
        <div className="flex items-center gap-1">
          <Button variant="outline" onClick={() => actualizar({ fecha: hoy() })} className="mr-1">
            Hoy
          </Button>
          <Button variant="ghost" size="icon" aria-label={`${unidad} anterior`} onClick={() => mover(-1)}>
            <ChevronLeft />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            aria-label={vista === 'dia' ? 'Día siguiente' : 'Semana siguiente'}
            onClick={() => mover(1)}
          >
            <ChevronRight />
          </Button>
        </div>

        <SelectorFecha fecha={fecha} vista={vista} alCambiar={(f) => actualizar({ fecha: f })} />

        <div className="flex w-full flex-wrap items-center gap-3 sm:ml-auto sm:w-auto">
          {!esMedico ? (
            <Select
              value={idDoctor ? String(idDoctor) : TODOS}
              onValueChange={(v) => actualizar({ doctor: v === TODOS ? null : Number(v) })}
            >
              <SelectTrigger className="h-8 min-w-52 flex-1 sm:flex-none" aria-label="Filtrar por doctor">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={TODOS}>Todos los doctores</SelectItem>
                {(doctores.data ?? []).map((d) => (
                  <SelectItem key={d.id} value={String(d.id)}>
                    {d.nombreCompleto}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          ) : null}

          <Tabs value={vista} onValueChange={(v) => actualizar({ vista: v as Vista })}>
            <TabsList aria-label="Vista de la agenda">
              <TabsTrigger value="dia" className="px-3">
                Día
              </TabsTrigger>
              <TabsTrigger value="semana" className="px-3">
                Semana
              </TabsTrigger>
            </TabsList>
          </Tabs>
        </div>
      </div>

      {citas.data ? <ResumenEstados citas={citas.data} /> : null}

      {citas.isPending ? (
        <EsqueletoAgenda vista={vista} />
      ) : citas.isError ? (
        <ErrorVista error={citas.error} alReintentar={() => citas.refetch()} />
      ) : vista === 'dia' ? (
        <VistaDia
          fecha={fecha}
          citas={citas.data}
          mostrarDoctor={!esMedico && !idDoctor}
          alAbrir={(c) => setIdAbierta(c.id)}
          alAgendarEn={
            puedeAgendar
              ? (horario) => setFormulario({ cita: null, iniciales: { fecha, horario, idDoctor } })
              : undefined
          }
        />
      ) : (
        <VistaSemana
          fecha={fecha}
          citas={citas.data}
          mostrarDoctor={!esMedico && !idDoctor}
          alAbrir={(c) => setIdAbierta(c.id)}
          alIrADia={(dia) => actualizar({ fecha: dia, vista: 'dia' })}
        />
      )}

      <DetalleCita
        cita={abierta}
        alCerrar={() => setIdAbierta(null)}
        puedeAgendar={puedeAgendar}
        alEditar={(c) => setFormulario({ cita: c })}
      />

      <FormularioCita
        abierto={formulario !== null}
        cita={formulario?.cita}
        iniciales={formulario?.iniciales}
        alCerrar={() => setFormulario(null)}
        alGuardar={(guardada) => {
          // Lleva la agenda al día de la cita para que se vea dónde quedó
          const dia = aFecha(guardada.fechaHora)
          if (vista === 'dia' ? !isSameDay(dia, fecha) : !dias.some((d) => isSameDay(d, dia))) {
            actualizar({ fecha: dia })
          }
        }}
      />
    </div>
  )
}

function SelectorFecha({ fecha, vista, alCambiar }: { fecha: Date; vista: Vista; alCambiar: (fecha: Date) => void }) {
  const [abierto, setAbierto] = useState(false)
  const semana = diasDeLaSemana(fecha)

  return (
    <Popover open={abierto} onOpenChange={setAbierto}>
      <PopoverTrigger asChild>
        <Button variant="ghost" className="h-auto gap-2 px-2 py-1 text-left" aria-label="Elegir fecha">
          <CalendarDays className="size-4 text-marea" aria-hidden="true" />
          <span className="font-heading text-lg font-semibold text-tinta" aria-live="polite">
            {tituloRango(fecha, vista)}
          </span>
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0" align="start">
        <Calendar
          mode="single"
          locale={es}
          weekStartsOn={1}
          selected={fecha}
          defaultMonth={fecha}
          modifiers={vista === 'semana' ? { semana } : undefined}
          modifiersClassNames={{ semana: 'bg-marea-suave rounded-none first:rounded-l-md last:rounded-r-md' }}
          onSelect={(dia) => {
            if (dia) alCambiar(dia)
            setAbierto(false)
          }}
        />
      </PopoverContent>
    </Popover>
  )
}

/** Conteo por estado del rango visible; también funciona como leyenda de los colores. */
function ResumenEstados({ citas }: { citas: Cita[] }) {
  const conteo = citas.reduce<Partial<Record<EstadoCita, number>>>((acumulado, c) => {
    acumulado[c.estado] = (acumulado[c.estado] ?? 0) + 1
    return acumulado
  }, {})

  return (
    <div className="-mt-2 flex flex-wrap items-center gap-x-5 gap-y-2 text-pizarra">
      <span className="font-medium text-tinta">{citas.length === 1 ? '1 cita' : `${citas.length} citas`}</span>
      {(Object.keys(ESTADOS_CITA) as EstadoCita[]).map((estado) => (
        <span key={estado} className={cn('flex items-center gap-1.5 text-xs', !conteo[estado] && 'opacity-60')}>
          <span className={cn('size-2 rounded-full', ESTADOS_CITA[estado].barra)} aria-hidden="true" />
          {ESTADOS_CITA[estado].texto}
          <span className="cifras font-medium text-tinta">{conteo[estado] ?? 0}</span>
        </span>
      ))}
    </div>
  )
}

function EsqueletoAgenda({ vista }: { vista: Vista }) {
  if (vista === 'semana') {
    return (
      <div className="grid gap-px overflow-hidden rounded-[10px] border border-linea bg-linea lg:grid-cols-7">
        {Array.from({ length: 7 }, (_, i) => (
          <div key={i} className="grid content-start gap-2 bg-card p-2 lg:min-h-80">
            <Skeleton className="h-7 w-16" />
            <Skeleton className="h-14" />
            <Skeleton className="h-14" />
          </div>
        ))}
      </div>
    )
  }
  return (
    <div className="grid gap-6 rounded-[10px] border border-linea bg-card p-4">
      {Array.from({ length: 6 }, (_, i) => (
        <div key={i} className="grid grid-cols-[3.75rem_1fr] items-center gap-3">
          <Skeleton className="h-4 w-10" />
          <Skeleton className={cn('h-12', i % 2 ? 'w-2/3' : 'w-full')} />
        </div>
      ))}
    </div>
  )
}
