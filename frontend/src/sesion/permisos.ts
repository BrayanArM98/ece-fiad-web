import { CalendarDays, ChartColumn, House, Stethoscope, UserCog, Users, type LucideIcon } from 'lucide-react'
import type { Rol } from '@/lib/tipos'

/**
 * Pantallas del frontend y los roles que las ven (specs/frontend-pantallas.md, "Rutas").
 * El menú lateral y las rutas protegidas se arman a partir de esta lista.
 */
export interface Pantalla {
  ruta: string
  titulo: string
  icono: LucideIcon
  roles: Rol[]
}

export const PERSONAL: Rol[] = ['Administrador', 'Recepcionista', 'Medico']

export const PANTALLAS: Pantalla[] = [
  { ruta: '/', titulo: 'Inicio', icono: House, roles: PERSONAL },
  { ruta: '/agenda', titulo: 'Agenda', icono: CalendarDays, roles: PERSONAL },
  { ruta: '/pacientes', titulo: 'Pacientes', icono: Users, roles: PERSONAL },
  { ruta: '/doctores', titulo: 'Doctores', icono: Stethoscope, roles: PERSONAL },
  { ruta: '/reportes', titulo: 'Reportes', icono: ChartColumn, roles: PERSONAL },
  { ruta: '/usuarios', titulo: 'Usuarios', icono: UserCog, roles: ['Administrador'] },
]

export const TEXTO_ROL: Record<Rol, string> = {
  Administrador: 'Administración',
  Recepcionista: 'Recepción',
  Medico: 'Médico',
  Paciente: 'Paciente',
}

export function puedeVer(rol: Rol | undefined, roles: Rol[]) {
  return rol !== undefined && roles.includes(rol)
}
