import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { createBrowserRouter } from 'react-router'
import { RouterProvider } from 'react-router/dom'
import { Estructura } from '@/componentes/Estructura'
import { Toaster } from '@/components/ui/sonner'
import { TooltipProvider } from '@/components/ui/tooltip'
import { ErrorApi } from '@/lib/api'
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

const enrutador = createBrowserRouter([
  { path: '/login', element: <PaginaLogin />, errorElement: <ErrorRuta /> },
  {
    element: <RutaProtegida />,
    errorElement: <ErrorRuta />,
    children: [
      {
        element: <Estructura />,
        children: [
          {
            index: true,
            element: (
              <SoloRoles roles={PERSONAL}>
                <PaginaInicio />
              </SoloRoles>
            ),
          },
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
