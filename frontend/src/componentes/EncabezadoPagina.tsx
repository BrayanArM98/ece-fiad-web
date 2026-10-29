import type { ReactNode } from 'react'

/**
 * Título de la página con su descripción y, a la derecha, la acción principal
 * (specs/frontend-diseno.md, "Estructura general").
 */
export function EncabezadoPagina({
  titulo,
  descripcion,
  accion,
}: {
  titulo: string
  descripcion?: ReactNode
  accion?: ReactNode
}) {
  return (
    <header className="flex flex-wrap items-end justify-between gap-4 border-b border-linea pb-5">
      <div>
        <h1 className="text-[1.75rem] font-semibold tracking-tight">{titulo}</h1>
        {descripcion ? <p className="mt-1 text-pizarra">{descripcion}</p> : null}
      </div>
      {accion}
    </header>
  )
}
