import { useQuery } from '@tanstack/react-query'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista, Vacio } from '@/componentes/EstadosVista'
import { EsqueletoIndicadores, RejillaIndicadores } from '@/componentes/Indicadores'
import { InsigniaEstado } from '@/componentes/InsigniaEstado'
import { Skeleton } from '@/components/ui/skeleton'
import { pedir } from '@/lib/api'
import { ESTADOS_CITA } from '@/lib/estados'
import { aFechaApi, formatearFechaLarga, formatearHora, mayuscula, nombreMes } from '@/lib/formato'
import type { Cita, ResumenGeneral } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { useSesion } from '@/sesion/SesionContext'

/**
 * Inicio (specs/frontend-pantallas.md, "Inicio"): indicadores del mes contra el mes anterior
 * y la agenda de hoy. La recepción no ve los indicadores porque no tiene acceso al resumen.
 */
export default function PaginaInicio() {
  const { usuario } = useSesion()
  const hoy = new Date()
  const verIndicadores = usuario?.rol === 'Administrador' || usuario?.rol === 'Medico'

  return (
    <div className="grid gap-10">
      <EncabezadoPagina titulo="Inicio" descripcion={mayuscula(formatearFechaLarga(hoy))} />
      {verIndicadores ? <FranjaIndicadores /> : null}
      <AgendaDeHoy esMedico={usuario?.rol === 'Medico'} />
    </div>
  )
}

// ============================================
// Indicadores del mes
// ============================================

function FranjaIndicadores() {
  const consulta = useQuery({
    queryKey: ['reportes', 'resumen', 'mes-actual'],
    queryFn: () => pedir<ResumenGeneral>('/api/Reportes/resumen'),
  })

  return (
    <section aria-labelledby="titulo-indicadores">
      <div className="mb-4 flex flex-wrap items-baseline justify-between gap-2">
        <h2 id="titulo-indicadores" className="text-lg font-semibold">
          {mayuscula(nombreMes(new Date()))}
        </h2>
        {consulta.data ? (
          <p className="text-pizarra">Comparado con {nombreMes(consulta.data.periodoAnterior.desde)}</p>
        ) : null}
      </div>

      {consulta.isPending ? (
        <EsqueletoIndicadores />
      ) : consulta.isError ? (
        <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
      ) : (
        <RejillaIndicadores indicadores={consulta.data.indicadores} comparadoCon="el mes anterior" />
      )}
    </section>
  )
}

// ============================================
// Agenda de hoy
// ============================================

function AgendaDeHoy({ esMedico }: { esMedico: boolean }) {
  const hoy = aFechaApi(new Date())
  const consulta = useQuery({
    queryKey: ['citas', { desde: hoy, hasta: hoy }],
    queryFn: () => pedir<Cita[]>('/api/Citas', { parametros: { desde: hoy, hasta: hoy } }),
    select: (citas) => [...citas].sort((a, b) => a.fechaHora.localeCompare(b.fechaHora)),
  })

  return (
    <section aria-labelledby="titulo-agenda">
      <div className="mb-4 flex items-baseline justify-between gap-2">
        <h2 id="titulo-agenda" className="text-lg font-semibold">
          {esMedico ? 'Tu agenda de hoy' : 'Agenda de hoy'}
        </h2>
        {consulta.data ? (
          <p className="text-pizarra">
            {consulta.data.length === 1 ? '1 cita' : `${consulta.data.length} citas`}
          </p>
        ) : null}
      </div>

      {consulta.isPending ? (
        <div className="grid gap-3">
          {Array.from({ length: 4 }, (_, i) => (
            <div key={i} className="grid grid-cols-[4.5rem_1fr] gap-4">
              <Skeleton className="h-5 w-12" />
              <Skeleton className="h-16" />
            </div>
          ))}
        </div>
      ) : consulta.isError ? (
        <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
      ) : consulta.data.length === 0 ? (
        <Vacio mensaje={esMedico ? 'No tienes citas para hoy.' : 'No hay citas agendadas para hoy.'} />
      ) : (
        <ol className="grid gap-2">
          {consulta.data.map((cita) => (
            <FilaAgenda key={cita.id} cita={cita} mostrarDoctor={!esMedico} />
          ))}
        </ol>
      )}
    </section>
  )
}

function FilaAgenda({ cita, mostrarDoctor }: { cita: Cita; mostrarDoctor: boolean }) {
  const cancelada = cita.estado === 'Cancelada'

  return (
    <li className="grid grid-cols-[4.5rem_1fr] items-stretch gap-4">
      <time dateTime={cita.fechaHora} className="cifras pt-3 font-heading text-base font-medium text-tinta">
        {formatearHora(cita.fechaHora)}
      </time>
      <div className="relative flex flex-wrap items-center gap-x-6 gap-y-1 overflow-hidden rounded-md border border-linea bg-card py-3 pr-4 pl-5">
        <span className={cn('absolute inset-y-0 left-0 w-1', ESTADOS_CITA[cita.estado].barra)} aria-hidden="true" />
        <div className={cn('min-w-48 flex-1', cancelada && 'text-pizarra line-through decoration-pizarra/40')}>
          <p className="font-medium">{cita.nombrePaciente}</p>
          <p className="text-pizarra">{cita.motivo || 'Sin motivo registrado'}</p>
        </div>
        {mostrarDoctor ? <p className="text-pizarra">{cita.nombreDoctor}</p> : null}
        <InsigniaEstado estado={cita.estado} />
      </div>
    </li>
  )
}
