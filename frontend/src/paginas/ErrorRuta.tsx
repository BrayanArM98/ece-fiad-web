import { Link, useRouteError } from 'react-router'
import { Button } from '@/components/ui/button'

/**
 * Se muestra si una pantalla falla al dibujarse, en lugar de la pantalla de error de React Router.
 * El detalle técnico queda en la consola del navegador.
 */
export default function ErrorRuta() {
  const error = useRouteError()
  console.error(error)

  return (
    <div className="grid min-h-svh place-content-center gap-4 px-6 text-center">
      <h1 className="text-[1.75rem] font-semibold">Esta pantalla no se pudo mostrar</h1>
      <p className="max-w-md text-pizarra">
        Ocurrió un error inesperado. Recarga la página; si vuelve a pasar, avisa a la administración del sistema.
      </p>
      <div className="flex justify-center gap-3">
        <Button onClick={() => window.location.reload()}>Recargar</Button>
        <Button variant="outline" asChild>
          <Link to="/">Ir al inicio</Link>
        </Button>
      </div>
    </div>
  )
}
