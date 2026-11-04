import { format } from 'date-fns'
import { es } from 'date-fns/locale'
import { Bar, BarChart, CartesianGrid, XAxis, YAxis } from 'recharts'
import { ChartContainer, ChartTooltip, ChartTooltipContent, type ChartConfig } from '@/components/ui/chart'
import { aFecha, formatearCifra } from '@/lib/formato'
import type { Conteo, PuntoSerieCitas } from '@/lib/tipos'
import { cn } from '@/lib/utils'

/**
 * Gráficas de los reportes. Los colores de estado son los del spec (Completada verde, Pendiente ámbar,
 * No asistió magenta, Cancelada gris) y siempre van con su texto: leyenda, etiqueta directa o tooltip.
 * Cancelada además lleva rayas, porque junto a No asistió no se distingue bien con daltonismo.
 */

// ============================================
// Barras horizontales con etiqueta directa
// ============================================

export function BarrasConteo({
  conteos,
  claseBarra,
  vacio = 'Sin datos en el periodo.',
  limite,
}: {
  conteos: Conteo[]
  /** Clase de color por etiqueta; por defecto, marea. */
  claseBarra?: (etiqueta: string) => string
  vacio?: string
  limite?: number
}) {
  const visibles = limite ? conteos.slice(0, limite) : conteos
  const maximo = Math.max(1, ...visibles.map((c) => c.cantidad))

  if (visibles.length === 0) return <p className="py-6 text-pizarra">{vacio}</p>

  return (
    <ul className="grid gap-3">
      {visibles.map((c) => (
        <li key={c.etiqueta} className="grid gap-1.5">
          <div className="flex items-baseline justify-between gap-3">
            <span className="min-w-0 truncate text-tinta" title={c.etiqueta}>
              {c.etiqueta}
            </span>
            <span className="cifras shrink-0 text-pizarra">
              <span className="font-medium text-tinta">{formatearCifra(c.cantidad)}</span> ·{' '}
              {formatearCifra(c.porcentaje, 'porcentaje')}
            </span>
          </div>
          <div className="h-2 overflow-hidden rounded-full bg-papel" aria-hidden="true">
            <div
              className={cn('h-full rounded-full', claseBarra ? claseBarra(c.etiqueta) : 'bg-marea')}
              style={{ width: `${(c.cantidad / maximo) * 100}%` }}
            />
          </div>
        </li>
      ))}
    </ul>
  )
}

/** Clase de color de la barra según el texto del estado que manda la API ("No asistió", "Completada"...). */
export function colorEstado(etiqueta: string) {
  switch (etiqueta) {
    case 'Pendiente':
      return 'bg-estado-pendiente'
    case 'Confirmada':
      return 'bg-estado-confirmada'
    case 'Completada':
      return 'bg-estado-completada'
    case 'No asistió':
      return 'bg-estado-noasistio'
    case 'Cancelada':
      return 'bg-[repeating-linear-gradient(135deg,var(--color-estado-cancelada)_0_3px,color-mix(in_oklab,var(--color-estado-cancelada),white_55%)_3px_6px)]'
    default:
      return 'bg-marea'
  }
}

// ============================================
// Citas en el tiempo: barras apiladas por resultado
// ============================================

const configuracionSerie = {
  completadas: { label: 'Completadas', color: 'var(--color-estado-completada)' },
  porAtender: { label: 'Por atender', color: 'var(--color-estado-pendiente)' },
  noAsistio: { label: 'No asistió', color: 'var(--color-estado-noasistio)' },
  canceladas: { label: 'Canceladas', color: 'var(--color-estado-cancelada)' },
} satisfies ChartConfig

export function GraficaSerieCitas({ serie, agrupacion }: { serie: PuntoSerieCitas[]; agrupacion: 'dia' | 'semana' }) {
  const etiquetaCorta = (valor: string) =>
    format(aFecha(valor), agrupacion === 'dia' ? 'd MMM' : "'sem' d MMM", { locale: es }).replace('.', '')
  const etiquetaLarga = (valor: string) =>
    agrupacion === 'dia'
      ? format(aFecha(valor), "EEEE d 'de' MMMM", { locale: es })
      : `Semana del ${format(aFecha(valor), "d 'de' MMMM", { locale: es })}`

  return (
    <div className="grid gap-3">
      <ChartContainer config={configuracionSerie} className="aspect-auto h-72 w-full">
        <BarChart data={serie} margin={{ top: 8, right: 4, left: -16, bottom: 0 }} barCategoryGap="20%">
          <defs>
            <pattern
              id="rayas-canceladas"
              patternUnits="userSpaceOnUse"
              width="6"
              height="6"
              patternTransform="rotate(45)"
            >
              <rect width="6" height="6" fill="var(--color-canceladas)" opacity="0.45" />
              <rect width="3" height="6" fill="var(--color-canceladas)" />
            </pattern>
          </defs>
          <CartesianGrid vertical={false} stroke="var(--color-linea)" />
          <XAxis
            dataKey="fecha"
            tickLine={false}
            axisLine={false}
            tickMargin={8}
            minTickGap={16}
            tickFormatter={etiquetaCorta}
          />
          <YAxis tickLine={false} axisLine={false} allowDecimals={false} width={40} />
          <ChartTooltip
            cursor={{ fill: 'var(--color-marea-suave)', opacity: 0.5 }}
            content={
              <ChartTooltipContent
                labelFormatter={(_, payload) => {
                  const fecha = payload?.[0]?.payload?.fecha as string | undefined
                  return fecha ? etiquetaLarga(fecha) : ''
                }}
              />
            }
          />
          <Bar
            dataKey="completadas"
            stackId="citas"
            fill="var(--color-completadas)"
            stroke="var(--color-card)"
            strokeWidth={1}
          />
          <Bar
            dataKey="porAtender"
            stackId="citas"
            fill="var(--color-porAtender)"
            stroke="var(--color-card)"
            strokeWidth={1}
          />
          <Bar
            dataKey="noAsistio"
            stackId="citas"
            fill="var(--color-noAsistio)"
            stroke="var(--color-card)"
            strokeWidth={1}
          />
          <Bar
            dataKey="canceladas"
            stackId="citas"
            fill="url(#rayas-canceladas)"
            stroke="var(--color-card)"
            strokeWidth={1}
            radius={[4, 4, 0, 0]}
          />
        </BarChart>
      </ChartContainer>
      {/* Leyenda en el mismo orden que la pila, de abajo hacia arriba */}
      <ul className="flex flex-wrap justify-center gap-x-5 gap-y-1 text-xs text-pizarra">
        {(
          [
            ['Completadas', 'bg-estado-completada'],
            ['Por atender', 'bg-estado-pendiente'],
            ['No asistió', 'bg-estado-noasistio'],
            ['Canceladas', colorEstado('Cancelada')],
          ] as const
        ).map(([texto, clase]) => (
          <li key={texto} className="flex items-center gap-1.5">
            <span className={cn('size-2.5 rounded-[3px]', clase)} aria-hidden="true" />
            {texto}
          </li>
        ))}
      </ul>
    </div>
  )
}
