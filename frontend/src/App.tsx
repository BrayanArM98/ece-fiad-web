import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { lazy, Suspense, type ReactNode } from 'react'
import { createBrowserRouter } from 'react-router'
import { RouterProvider } from 'react-router/dom'
import { Estructura } from '@/componentes/Estructura'
import { Skeleton } from '@/components/ui/skeleton'
import { Toaster } from '@/components/ui/sonner'
import { TooltipProvider } from '@/components/ui/tooltip'
import { ErrorApi } from '@/lib/api'
import type { Rol } from '@/lib/tipos'
import ErrorRuta from '@/paginas/ErrorRuta'
import PaginaInicio from '@/paginas/Inicio'
import PaginaLogin from '@/paginas/Login'
import PaginaNoEncontrada from '@/paginas/NoEncontrada'
import { PERSONAL } from '@/sesion/permisos'
import { RutaProtegida, SoloRoles } from '@/sesion/Rutas'
import { SesionProvider } from '@/sesion/SesionContext'

const clienteConsultas = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 30_000,
      refetchOnWindowFocus: false,
      // No se reintenta lo que no va a cambiar: datos inválidos, sin sesión, sin permiso o inexistente
      retry: (intentos, error) =>
        !(error instanceof ErrorApi && [400, 401, 403, 404].includes(error.status)) && intentos < 2,
    },
  },
})

// Cada pantalla se descarga la primera vez que se abre; así el inicio de sesión carga rápido
const PaginaAgenda = lazy(() => import('@/paginas/Agenda'))
const PaginaPacientes = lazy(() => import('@/paginas/Pacientes'))
const PaginaExpediente = lazy(() => import('@/paginas/Expediente'))
const PaginaDoctores = lazy(() => import('@/paginas/Doctores'))
const PaginaReportes = lazy(() => import('@/paginas/Reportes'))
const PaginaUsuarios = lazy(() => import('@/paginas/Usuarios'))

const conRoles = (roles: Rol[], pagina: ReactNode) => (
  <SoloRoles roles={roles}>
    <Suspense fallback={<CargandoPantalla />}>{pagina}</Suspense>
  </SoloRoles>
)

function CargandoPantalla() {
  return (
    <div className="grid gap-6" aria-busy="true" aria-label="Cargando">
      <Skeleton className="h-9 w-56" />
      <Skeleton className="h-80" />
    </div>
  )
}

const enrutador = createBrowserRouter([
  { path: '/login', element: <PaginaLogin />, errorElement: <ErrorRuta /> },
  {
    element: <RutaProtegida />,
    errorElement: <ErrorRuta />,
    children: [
      {
        element: <Estructura />,
        children: [
          { index: true, element: conRoles(PERSONAL, <PaginaInicio />) },
          { path: 'agenda', element: conRoles(PERSONAL, <PaginaAgenda />) },
          { path: 'pacientes', element: conRoles(PERSONAL, <PaginaPacientes />) },
          { path: 'pacientes/:id', element: conRoles(PERSONAL, <PaginaExpediente />) },
          { path: 'doctores', element: conRoles(PERSONAL, <PaginaDoctores />) },
          { path: 'reportes', element: conRoles(PERSONAL, <PaginaReportes />) },
          { path: 'usuarios', element: conRoles(['Administrador'], <PaginaUsuarios />) },
        ],
      },
    ],
  },
  { path: '*', element: <PaginaNoEncontrada /> },
])

export default function App() {
  return (
    <QueryClientProvider client={clienteConsultas}>
      <SesionProvider>
        <TooltipProvider>
          <RouterProvider router={enrutador} />
          <Toaster position="top-right" richColors />
        </TooltipProvider>
      </SesionProvider>
    </QueryClientProvider>
  )
}
