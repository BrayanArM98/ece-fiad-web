import { ESTADOS_CITA } from '@/lib/estados'
import type { EstadoCita } from '@/lib/tipos'
import { cn } from '@/lib/utils'

export function InsigniaEstado({ estado, className }: { estado: EstadoCita; className?: string }) {
  const meta = ESTADOS_CITA[estado]
  return (
    <span className={cn('inline-flex h-6 items-center rounded-full px-2.5 text-xs font-medium', meta.insignia, className)}>
      {meta.texto}
    </span>
  )
}
