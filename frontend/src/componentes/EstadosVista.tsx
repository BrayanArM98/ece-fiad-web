import { CircleAlert } from 'lucide-react'
import type { ReactNode } from 'react'
import { Button } from '@/components/ui/button'
import { mensajeDeError } from '@/lib/api'

/**
 * Estados de vacío y error de las pantallas (specs/frontend-diseno.md, "Estados de cada pantalla").
 * El de carga lo resuelve cada pantalla con un esqueleto con la forma de su contenido.
 */
export function Vacio({ mensaje, accion }: { mensaje: string; accion?: ReactNode }) {
  return (
    <div className="flex flex-col items-start gap-3 rounded-[10px] border border-dashed border-linea px-6 py-10">
      <p className="text-pizarra">{mensaje}</p>
      {accion}
    </div>
  )
}

export function ErrorVista({ error, alReintentar }: { error: unknown; alReintentar?: () => void }) {
  return (
    <div role="alert" className="flex flex-col items-start gap-3 rounded-[10px] border border-coral/30 bg-coral-suave px-6 py-6">
      <p className="flex items-center gap-2 font-medium text-coral">
        <CircleAlert className="size-4" aria-hidden="true" />
        {mensajeDeError(error)}
      </p>
      {alReintentar ? (
        <Button variant="outline" size="sm" onClick={alReintentar}>
          Reintentar
        </Button>
      ) : null}
    </div>
  )
}
