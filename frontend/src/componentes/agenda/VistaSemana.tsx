import { format, isSameDay, isToday, isWeekend } from 'date-fns'
import { es } from 'date-fns/locale'
import { ESTADOS_CITA } from '@/lib/estados'
import { aFecha, formatearHora, mayuscula } from '@/lib/formato'
import type { Cita } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { diasDeLaSemana } from './fechas'

/**
 * Semana de lunes a domingo. En pantallas anchas es una columna por día;
 * en pantallas angostas los días se apilan uno debajo del otro.
 */
export function VistaSemana({
  fecha,
  citas,
  mostrarDoctor,
  alAbrir,
  alIrADia,
}: {
  fecha: Date
  citas: Cita[]
  mostrarDoctor: boolean
  alAbrir: (cita: Cita) => void
  alIrADia: (dia: Date) => void
}) {
  const dias = diasDeLaSemana(fecha)

  return (
    <div className="grid gap-px overflow-hidden rounded-[10px] border border-linea bg-linea lg:grid-cols-7">
      {dias.map((dia) => {
        const delDia = citas.filter((c) => isSameDay(aFecha(c.fechaHora), dia))
        const esHoy = isToday(dia)

        return (
          <section
            key={dia.toISOString()}
            aria-label={mayuscula(format(dia, "EEEE d 'de' MMMM", { locale: es }))}
            className={cn('flex min-w-0 flex-col gap-2 p-2 lg:min-h-80', isWeekend(dia) ? 'bg-papel' : 'bg-card')}
          >
            <button
              type="button"
              onClick={() => alIrADia(dia)}
              title="Ver el día"
              className="flex items-center gap-2 rounded-md px-1.5 py-1 text-left hover:bg-muted focus-visible:ring-3 focus-visible:ring-ring/50 focus-visible:outline-none"
            >
              <span className={cn('text-xs font-medium tracking-wide uppercase', esHoy ? 'text-marea' : 'text-pizarra')}>
                {format(dia, 'EEE', { locale: es }).replace('.', '')}
              </span>
              <span
                className={cn(
                  'cifras grid size-7 place-items-center rounded-full font-heading text-base font-semibold',
                  esHoy ? 'bg-marea text-white' : 'text-tinta',
                )}
              >
                {format(dia, 'd')}
              </span>
              <span className="ml-auto text-xs text-pizarra">
                {delDia.length === 0 ? '' : delDia.length === 1 ? '1 cita' : `${delDia.length} citas`}
              </span>
            </button>

            {delDia.length === 0 ? (
              <p className="px-1.5 pb-1 text-xs text-pizarra lg:hidden">Sin citas</p>
            ) : (
              <ol className="grid gap-1.5">
                {delDia.map((cita) => (
                  <li key={cita.id}>
                    <TarjetaCita cita={cita} mostrarDoctor={mostrarDoctor} alAbrir={alAbrir} />
                  </li>
                ))}
              </ol>
            )}
          </section>
        )
      })}
    </div>
  )
}

function TarjetaCita({
  cita,
  mostrarDoctor,
  alAbrir,
}: {
  cita: Cita
  mostrarDoctor: boolean
  alAbrir: (cita: Cita) => void
}) {
  const estado = ESTADOS_CITA[cita.estado]
  const cancelada = cita.estado === 'Cancelada'

  return (
    <button
      type="button"
      onClick={() => alAbrir(cita)}
      className={cn(
        'relative grid w-full gap-0.5 overflow-hidden rounded-md border border-linea py-1.5 pr-2 pl-3 text-left transition-colors hover:border-marea focus-visible:ring-3 focus-visible:ring-ring/50 focus-visible:outline-none',
        cancelada ? 'bg-papel' : 'bg-card',
      )}
    >
      <span className={cn('absolute inset-y-0 left-0 w-1', estado.barra)} aria-hidden="true" />
      <span className="flex items-baseline justify-between gap-2">
        <span className="cifras text-xs font-semibold text-tinta">{formatearHora(cita.fechaHora)}</span>
        <span className="truncate text-[0.6875rem] text-pizarra">{estado.texto}</span>
      </span>
      <span className={cn('truncate text-[0.8125rem] font-medium', cancelada && 'text-pizarra line-through')}>
        {cita.nombrePaciente}
      </span>
      {mostrarDoctor ? <span className="truncate text-xs text-pizarra">{cita.nombreDoctor}</span> : null}
    </button>
  )
}
