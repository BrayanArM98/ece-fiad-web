import { ShieldAlert } from 'lucide-react'
import type { ReactNode } from 'react'
import { Navigate, Outlet, useLocation } from 'react-router'
import type { Rol } from '@/lib/tipos'
import { puedeVer } from '@/sesion/permisos'
import { useSesion } from '@/sesion/SesionContext'

/** Sin sesión lleva a /login y recuerda la ruta pedida para regresar después de entrar. */
export function RutaProtegida() {
  const { usuario } = useSesion()
  const ubicacion = useLocation()

  if (!usuario) return <Navigate to="/login" replace state={{ desde: ubicacion.pathname + ubicacion.search }} />
  return <Outlet />
}

/** Muestra la pantalla solo a los roles indicados. */
export function SoloRoles({ roles, children }: { roles: Rol[]; children: ReactNode }) {
  const { usuario } = useSesion()
  if (puedeVer(usuario?.rol, roles)) return children

  return (
    <div className="flex max-w-lg items-start gap-3 rounded-[10px] border border-linea bg-card p-6">
      <ShieldAlert className="mt-0.5 size-5 text-coral" aria-hidden="true" />
      <div>
        <h1 className="text-lg font-semibold">No tienes acceso a esta sección</h1>
        <p className="mt-1 text-pizarra">Tu rol no incluye esta pantalla. Si la necesitas, pide acceso a la administración.</p>
      </div>
    </div>
  )
}
