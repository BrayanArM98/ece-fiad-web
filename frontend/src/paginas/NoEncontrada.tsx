import { Link } from 'react-router'
import { Button } from '@/components/ui/button'

export default function PaginaNoEncontrada() {
  return (
    <div className="grid min-h-svh place-content-center gap-4 px-6 text-center">
      <h1 className="text-[1.75rem] font-semibold">Esta página no existe</h1>
      <p className="text-pizarra">Revisa la dirección o regresa al inicio.</p>
      <div>
        <Button asChild>
          <Link to="/">Ir al inicio</Link>
        </Button>
      </div>
    </div>
  )
}
