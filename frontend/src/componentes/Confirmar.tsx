import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from '@/components/ui/alert-dialog'

/** Confirmación antes de una acción destructiva; se queda abierta hasta que la API responde. */
export function Confirmar({
  abierto,
  alCambiar,
  titulo,
  descripcion,
  textoConfirmar,
  textoCancelar = 'Cancelar',
  pendiente,
  alConfirmar,
}: {
  abierto: boolean
  alCambiar: (abierto: boolean) => void
  titulo: string
  descripcion: string
  textoConfirmar: string
  textoCancelar?: string
  pendiente: boolean
  alConfirmar: () => void
}) {
  return (
    <AlertDialog open={abierto} onOpenChange={alCambiar}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>{titulo}</AlertDialogTitle>
          <AlertDialogDescription>{descripcion}</AlertDialogDescription>
        </AlertDialogHeader>
        <AlertDialogFooter>
          <AlertDialogCancel disabled={pendiente}>{textoCancelar}</AlertDialogCancel>
          <AlertDialogAction
            variant="destructive"
            disabled={pendiente}
            onClick={(evento) => {
              evento.preventDefault()
              alConfirmar()
            }}
          >
            {pendiente ? 'Un momento…' : textoConfirmar}
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  )
}
