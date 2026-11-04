import type { ComponentProps } from 'react'
import { TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { cn } from '@/lib/utils'

/**
 * Pestañas tipo carpeta del expediente (specs/frontend-diseno.md, "Expediente"):
 * la pestaña activa se une con el contenido, como la ceja de un folder.
 */
export function ListaCarpeta({ className, ...props }: ComponentProps<typeof TabsList>) {
  return (
    <TabsList
      className={cn(
        'h-auto w-full justify-start gap-1 overflow-x-auto overflow-y-hidden rounded-none border-b border-linea bg-transparent p-0',
        className,
      )}
      {...props}
    />
  )
}

export function PestanaCarpeta({ className, ...props }: ComponentProps<typeof TabsTrigger>) {
  return (
    <TabsTrigger
      className={cn(
        'relative -mb-px h-10 flex-none rounded-t-lg rounded-b-none border border-transparent px-4 text-pizarra hover:text-tinta data-active:border-linea data-active:border-b-card data-active:bg-card data-active:text-tinta data-active:shadow-none!',
        className,
      )}
      {...props}
    />
  )
}

export function ContenidoCarpeta({ className, ...props }: ComponentProps<typeof TabsContent>) {
  return (
    <TabsContent
      className={cn('-mt-2 rounded-b-[10px] border border-t-0 border-linea bg-card p-5 sm:p-6', className)}
      {...props}
    />
  )
}
