import { ArrowDownRight, ArrowUpRight } from 'lucide-react'
import { Skeleton } from '@/components/ui/skeleton'
import { formatearCifra } from '@/lib/formato'
import type { Indicador } from '@/lib/tipos'
import { cn } from '@/lib/utils'

/** Si subir es bueno (verde) o malo (coral) para cada indicador. */
const SUBIR_ES_BUENO: Record<string, boolean> = {
  citas: true,
  asistencia: true,
  cancelaciones: false,
  pacientesAtendidos: true,
  evoluciones: true,
  pacientesNuevos: true,
}

const REJILLA =
  'grid grid-cols-2 gap-px overflow-hidden rounded-[10px] border border-linea bg-linea sm:grid-cols-3 xl:grid-cols-6'

/** Franja de indicadores con su variación contra el periodo anterior (Inicio y reporte Resumen). */
export function RejillaIndicadores({ indicadores, comparadoCon }: { indicadores: Indicador[]; comparadoCon: string }) {
  return (
    <dl className={REJILLA}>
      {indicadores.map((indicador) => (
        <CeldaIndicador key={indicador.clave} indicador={indicador} comparadoCon={comparadoCon} />
      ))}
    </dl>
  )
}

export function EsqueletoIndicadores() {
  return (
    <div className={REJILLA}>
      {Array.from({ length: 6 }, (_, i) => (
        <div key={i} className="grid gap-3 bg-card p-5">
          <Skeleton className="h-4 w-24" />
          <Skeleton className="h-8 w-16" />
          <Skeleton className="h-4 w-28" />
        </div>
      ))}
    </div>
  )
}

function CeldaIndicador({ indicador, comparadoCon }: { indicador: Indicador; comparadoCon: string }) {
  const { variacion, valorAnterior, formato } = indicador
  const subio = (variacion ?? 0) > 0
  const bajo = (variacion ?? 0) < 0
  const esBueno = SUBIR_ES_BUENO[indicador.clave] ? subio : bajo
  const esMalo = SUBIR_ES_BUENO[indicador.clave] ? bajo : subio
  const Flecha = subio ? ArrowUpRight : ArrowDownRight

  const cambio =
    variacion === null
      ? valorAnterior === null
        ? 'Sin datos para comparar'
        : `${mayusculaInicial(comparadoCon)}: ${formatearCifra(valorAnterior, formato)}`
      : variacion === 0
        ? `Igual que ${comparadoCon}`
        : `${formatearCifra(Math.abs(variacion))}${formato === 'porcentaje' ? ' pts' : '%'} ${subio ? 'más' : 'menos'}`

  return (
    <div className="grid content-start gap-1 bg-card p-5">
      <dt className="text-pizarra">{indicador.titulo}</dt>
      <dd className="font-heading text-[2rem] leading-tight font-semibold text-tinta">
        {formatearCifra(indicador.valor, formato)}
      </dd>
      <dd
        className={cn(
          'flex items-center gap-1 text-xs',
          esBueno && 'text-estado-completada',
          esMalo && 'text-coral',
          !esBueno && !esMalo && 'text-pizarra',
        )}
      >
        {variacion ? <Flecha className="size-3.5" aria-hidden="true" /> : null}
        {cambio}
      </dd>
    </div>
  )
}

const mayusculaInicial = (texto: string) => texto.charAt(0).toUpperCase() + texto.slice(1)
