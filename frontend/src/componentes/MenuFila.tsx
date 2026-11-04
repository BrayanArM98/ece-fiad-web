import { Ellipsis, type LucideIcon } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'

export interface AccionFila {
  texto: string
  icono: LucideIcon
  alElegir: () => void
  destructiva?: boolean
  deshabilitada?: boolean
}

/** Menú "…" con las acciones de una fila; las destructivas van al final, separadas. */
export function MenuFila({ etiqueta, acciones }: { etiqueta: string; acciones: AccionFila[] }) {
  const normales = acciones.filter((a) => !a.destructiva)
  const destructivas = acciones.filter((a) => a.destructiva)
  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button variant="ghost" size="icon" aria-label={etiqueta} onClick={(e) => e.stopPropagation()}>
          <Ellipsis />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent align="end" className="min-w-40" onClick={(e) => e.stopPropagation()}>
        {normales.map(({ texto, icono: Icono, alElegir, deshabilitada }) => (
          <DropdownMenuItem key={texto} onSelect={alElegir} disabled={deshabilitada}>
            <Icono aria-hidden="true" />
            {texto}
          </DropdownMenuItem>
        ))}
        {normales.length > 0 && destructivas.length > 0 ? <DropdownMenuSeparator /> : null}
        {destructivas.map(({ texto, icono: Icono, alElegir, deshabilitada }) => (
          <DropdownMenuItem key={texto} variant="destructive" onSelect={alElegir} disabled={deshabilitada}>
            <Icono aria-hidden="true" />
            {texto}
          </DropdownMenuItem>
        ))}
      </DropdownMenuContent>
    </DropdownMenu>
  )
}
