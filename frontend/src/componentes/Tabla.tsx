import { ChevronLeft, ChevronRight, Search, X } from 'lucide-react'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Skeleton } from '@/components/ui/skeleton'
import { cn } from '@/lib/utils'

/** Piezas comunes de las pantallas de lista: búsqueda, paginación, estado y esqueleto. */

export function Buscador({
  valor,
  alCambiar,
  etiqueta,
  className,
}: {
  valor: string
  alCambiar: (valor: string) => void
  etiqueta: string
  className?: string
}) {
  return (
    <div className={cn('relative w-full sm:max-w-sm', className)}>
      <Search className="pointer-events-none absolute top-1/2 left-3 size-4 -translate-y-1/2 text-pizarra" aria-hidden="true" />
      <Input
        type="search"
        value={valor}
        onChange={(e) => alCambiar(e.target.value)}
        placeholder={etiqueta}
        aria-label={etiqueta}
        className="h-9 bg-card pr-9 pl-9 [&::-webkit-search-cancel-button]:hidden"
      />
      {valor ? (
        <button
          type="button"
          onClick={() => alCambiar('')}
          aria-label="Limpiar búsqueda"
          className="absolute top-1/2 right-2 grid size-6 -translate-y-1/2 place-items-center rounded text-pizarra hover:text-tinta"
        >
          <X className="size-4" />
        </button>
      ) : null}
    </div>
  )
}

export const POR_PAGINA = 12

export function Paginacion({
  pagina,
  total,
  alCambiar,
  porPagina = POR_PAGINA,
  nombre,
}: {
  pagina: number
  total: number
  alCambiar: (pagina: number) => void
  porPagina?: number
  /** "pacientes", "doctores"... */
  nombre: string
}) {
  const paginas = Math.max(1, Math.ceil(total / porPagina))
  const desde = total === 0 ? 0 : (pagina - 1) * porPagina + 1
  const hasta = Math.min(total, pagina * porPagina)

  return (
    <div className="flex flex-wrap items-center justify-between gap-3 text-pizarra">
      <p aria-live="polite">
        {total === 0 ? `Sin ${nombre}` : `${desde}–${hasta} de ${total} ${nombre}`}
      </p>
      {paginas > 1 ? (
        <div className="flex items-center gap-1">
          <Button
            variant="outline"
            size="icon"
            aria-label="Página anterior"
            disabled={pagina <= 1}
            onClick={() => alCambiar(pagina - 1)}
          >
            <ChevronLeft />
          </Button>
          <span className="cifras px-2 text-tinta">
            {pagina} / {paginas}
          </span>
          <Button
            variant="outline"
            size="icon"
            aria-label="Página siguiente"
            disabled={pagina >= paginas}
            onClick={() => alCambiar(pagina + 1)}
          >
            <ChevronRight />
          </Button>
        </div>
      ) : null}
    </div>
  )
}

export function paginar<T>(elementos: T[], pagina: number, porPagina = POR_PAGINA) {
  return elementos.slice((pagina - 1) * porPagina, pagina * porPagina)
}

export function InsigniaActivo({ activo, textoInactivo = 'Inactivo' }: { activo: boolean; textoInactivo?: string }) {
  return (
    <span
      className={cn(
        'inline-flex h-6 items-center gap-1.5 rounded-full px-2.5 text-xs font-medium',
        activo ? 'bg-estado-completada/10 text-estado-completada' : 'bg-estado-cancelada/10 text-estado-cancelada',
      )}
    >
      <span className={cn('size-1.5 rounded-full', activo ? 'bg-estado-completada' : 'bg-estado-cancelada')} aria-hidden="true" />
      {activo ? 'Activo' : textoInactivo}
    </span>
  )
}

export function EsqueletoTabla({ filas = 6, columnas = 4 }: { filas?: number; columnas?: number }) {
  return (
    <div className="overflow-hidden rounded-[10px] border border-linea bg-card">
      <div className="border-b border-linea bg-papel px-4 py-3">
        <Skeleton className="h-4 w-40" />
      </div>
      {Array.from({ length: filas }, (_, i) => (
        <div key={i} className="flex gap-6 border-b border-linea px-4 py-4 last:border-b-0">
          {Array.from({ length: columnas }, (_, j) => (
            <Skeleton key={j} className={cn('h-4', j === 0 ? 'w-48' : 'hidden w-24 sm:block')} />
          ))}
        </div>
      ))}
    </div>
  )
}

/** Clases del contenedor de una tabla: borde, fondo blanco y scroll horizontal si no cabe. */
export const CONTENEDOR_TABLA = 'overflow-hidden rounded-[10px] border border-linea bg-card'
export const ENCABEZADO_TABLA = 'bg-papel text-xs font-medium text-pizarra'
