import { Outlet } from 'react-router'
import { BarraLateral } from '@/componentes/BarraLateral'
import { SimboloMarca } from '@/componentes/Marca'
import { SidebarInset, SidebarProvider, SidebarTrigger } from '@/components/ui/sidebar'

/** Estructura de las pantallas con sesión: barra lateral a la izquierda y contenido a la derecha. */
export function Estructura() {
  return (
    <SidebarProvider>
      <BarraLateral />
      <SidebarInset className="bg-background">
        {/* Encabezado solo en pantallas angostas, donde la barra lateral se oculta */}
        <div className="flex items-center gap-2 border-b border-linea bg-card px-4 py-2 md:hidden">
          <SidebarTrigger aria-label="Abrir menú" />
          <SimboloMarca className="size-6 text-marea" />
          <span className="font-heading font-semibold">ECE-FIAD</span>
        </div>
        <main className="mx-auto w-full max-w-[1280px] px-4 py-8 sm:px-8">
          <Outlet />
        </main>
      </SidebarInset>
    </SidebarProvider>
  )
}
