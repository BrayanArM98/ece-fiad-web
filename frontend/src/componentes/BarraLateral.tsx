import { ChevronsUpDown, LogOut } from 'lucide-react'
import { NavLink, useLocation } from 'react-router'
import { SimboloMarca } from '@/componentes/Marca'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarGroup,
  SidebarGroupContent,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
} from '@/components/ui/sidebar'
import { PANTALLAS, TEXTO_ROL, puedeVer } from '@/sesion/permisos'
import { useSesion } from '@/sesion/SesionContext'

function iniciales(nombre: string) {
  return nombre
    .replace(/^(Dra?\.|Lic\.)\s+/i, '')
    .split(/\s+/)
    .slice(0, 2)
    .map((parte) => parte.charAt(0).toUpperCase())
    .join('')
}

/**
 * Barra lateral: marca, navegación del rol y usuario de la sesión.
 * En pantallas angostas shadcn/ui la convierte en un panel desplegable.
 */
export function BarraLateral() {
  const { usuario, cerrarSesion } = useSesion()
  const { pathname } = useLocation()
  const pantallas = PANTALLAS.filter((p) => puedeVer(usuario?.rol, p.roles))

  return (
    <Sidebar>
      <SidebarHeader className="border-b border-sidebar-border px-4 py-4">
        <div className="flex items-center gap-3">
          <SimboloMarca className="text-marea" />
          <div className="leading-tight">
            <p className="font-heading text-base font-semibold text-tinta">ECE-FIAD</p>
            <p className="text-xs text-pizarra">Expediente clínico</p>
          </div>
        </div>
      </SidebarHeader>

      <SidebarContent>
        <SidebarGroup>
          <SidebarGroupContent>
            <SidebarMenu>
              {pantallas.map(({ ruta, titulo, icono: Icono }) => {
                const activa = ruta === '/' ? pathname === '/' : pathname.startsWith(ruta)
                return (
                  <SidebarMenuItem key={ruta}>
                    <SidebarMenuButton asChild isActive={activa} className="h-9">
                      <NavLink to={ruta} end={ruta === '/'}>
                        <Icono aria-hidden="true" />
                        <span>{titulo}</span>
                      </NavLink>
                    </SidebarMenuButton>
                  </SidebarMenuItem>
                )
              })}
            </SidebarMenu>
          </SidebarGroupContent>
        </SidebarGroup>
      </SidebarContent>

      {usuario ? (
        <SidebarFooter className="border-t border-sidebar-border">
          <DropdownMenu>
            <DropdownMenuTrigger asChild>
              <SidebarMenuButton size="lg" className="data-open:bg-sidebar-accent">
                <span className="grid size-8 shrink-0 place-items-center rounded-md bg-marea-suave font-heading text-xs font-semibold text-marea-profunda">
                  {iniciales(usuario.nombreCompleto)}
                </span>
                <span className="grid flex-1 leading-tight">
                  <span className="truncate font-medium">{usuario.nombreCompleto}</span>
                  <span className="truncate text-xs text-pizarra">{TEXTO_ROL[usuario.rol]}</span>
                </span>
                <ChevronsUpDown className="ml-auto" aria-hidden="true" />
              </SidebarMenuButton>
            </DropdownMenuTrigger>
            <DropdownMenuContent side="top" align="start" className="w-(--radix-dropdown-menu-trigger-width) min-w-56">
              <DropdownMenuLabel className="font-normal">
                <span className="block font-medium text-tinta">{usuario.nombreCompleto}</span>
                <span className="block text-xs text-pizarra">{usuario.email}</span>
              </DropdownMenuLabel>
              <DropdownMenuSeparator />
              <DropdownMenuItem onSelect={() => cerrarSesion()}>
                <LogOut aria-hidden="true" />
                Cerrar sesión
              </DropdownMenuItem>
            </DropdownMenuContent>
          </DropdownMenu>
        </SidebarFooter>
      ) : null}
    </Sidebar>
  )
}
