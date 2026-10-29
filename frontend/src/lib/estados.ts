import type { EstadoCita } from '@/lib/tipos'

/**
 * Texto y color de cada estado de cita (specs/frontend-diseno.md, "Paleta").
 * El color siempre va acompañado del texto, nunca solo.
 */
export const ESTADOS_CITA: Record<EstadoCita, { texto: string; barra: string; insignia: string }> = {
  Pendiente: {
    texto: 'Pendiente',
    barra: 'bg-estado-pendiente',
    insignia: 'bg-estado-pendiente/10 text-estado-pendiente',
  },
  Confirmada: {
    texto: 'Confirmada',
    barra: 'bg-estado-confirmada',
    insignia: 'bg-estado-confirmada/10 text-estado-confirmada',
  },
  Completada: {
    texto: 'Completada',
    barra: 'bg-estado-completada',
    insignia: 'bg-estado-completada/10 text-estado-completada',
  },
  NoAsistio: {
    texto: 'No asistió',
    barra: 'bg-estado-noasistio',
    insignia: 'bg-estado-noasistio/10 text-estado-noasistio',
  },
  Cancelada: {
    texto: 'Cancelada',
    barra: 'bg-estado-cancelada',
    insignia: 'bg-estado-cancelada/10 text-estado-cancelada',
  },
}
