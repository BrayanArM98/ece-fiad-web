import { CircleAlert } from 'lucide-react'
import type { ReactNode } from 'react'
import { Button } from '@/components/ui/button'
import { DialogFooter } from '@/components/ui/dialog'
import { Label } from '@/components/ui/label'
import { ErrorApi } from '@/lib/api'

/**
 * Piezas comunes de los formularios en diálogo (specs/frontend-diseno.md, "Formularios"):
 * etiqueta arriba, error debajo en coral y el aviso de la API arriba del formulario.
 */

export function Campo({
  etiqueta,
  htmlFor,
  error,
  opcional,
  ayuda,
  className,
  children,
}: {
  etiqueta: string
  htmlFor: string
  error?: string
  opcional?: boolean
  ayuda?: string
  className?: string
  children: ReactNode
}) {
  return (
    <div className={['grid content-start gap-1.5', className].filter(Boolean).join(' ')}>
      <Label htmlFor={htmlFor} className="font-medium text-tinta">
        {etiqueta}
        {opcional ? <span className="font-normal text-pizarra">(opcional)</span> : null}
      </Label>
      {children}
      {error ? (
        <p id={`${htmlFor}-error`} className="text-xs text-coral">
          {error}
        </p>
      ) : ayuda ? (
        <p className="text-xs text-pizarra">{ayuda}</p>
      ) : null}
    </div>
  )
}

/** Atributos de accesibilidad de un control con error. */
export const conError = (id: string, error?: string) => ({
  'aria-invalid': error ? true : undefined,
  'aria-describedby': error ? `${id}-error` : undefined,
})

export function AvisoError({ error, titulo = 'Revisa los datos:' }: { error: unknown; titulo?: string }) {
  const lista = error instanceof ErrorApi && error.errores.length > 0 ? error.errores : null
  const mensaje = error instanceof ErrorApi ? (lista ? titulo : error.message) : 'Ocurrió un error inesperado.'
  return (
    <div role="alert" className="flex gap-2 rounded-md border border-coral/30 bg-coral-suave px-3 py-2.5 text-coral">
      <CircleAlert className="mt-0.5 size-4 shrink-0" aria-hidden="true" />
      <div className="grid gap-1">
        <p className="font-medium">{mensaje}</p>
        {lista ? (
          <ul className="list-disc pl-4">
            {lista.map((e) => (
              <li key={e}>{e}</li>
            ))}
          </ul>
        ) : null}
      </div>
    </div>
  )
}

export function PieFormulario({
  enviando,
  textoEnviar,
  alCancelar,
}: {
  enviando: boolean
  textoEnviar: string
  alCancelar: () => void
}) {
  return (
    <DialogFooter className="m-0 rounded-b-xl border-t border-linea bg-papel p-4 px-6">
      <Button type="button" variant="outline" onClick={alCancelar} disabled={enviando}>
        Cancelar
      </Button>
      <Button type="submit" disabled={enviando}>
        {enviando ? 'Guardando…' : textoEnviar}
      </Button>
    </DialogFooter>
  )
}
