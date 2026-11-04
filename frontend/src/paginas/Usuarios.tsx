import { formatDistanceToNow } from 'date-fns'
import { es } from 'date-fns/locale'
import { Pencil, Trash2, UserPlus } from 'lucide-react'
import { useState } from 'react'
import { Link } from 'react-router'
import { toast } from 'sonner'
import { Confirmar } from '@/componentes/Confirmar'
import { useListaDoctores } from '@/componentes/doctores/consultas'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista, Vacio } from '@/componentes/EstadosVista'
import { MenuFila } from '@/componentes/MenuFila'
import { usePacientes } from '@/componentes/pacientes/consultas'
import { Buscador, CONTENEDOR_TABLA, ENCABEZADO_TABLA, EsqueletoTabla, InsigniaActivo } from '@/componentes/Tabla'
import { useEliminarUsuario, useUsuarios } from '@/componentes/usuarios/consultas'
import { FormularioUsuario } from '@/componentes/usuarios/FormularioUsuario'
import { Button } from '@/components/ui/button'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { mensajeDeError } from '@/lib/api'
import { normalizar, ROLES } from '@/lib/catalogos'
import type { Rol, Usuario } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { TEXTO_ROL } from '@/sesion/permisos'
import { useSesion } from '@/sesion/SesionContext'

const TODOS = 'todos'

const COLOR_ROL: Record<Rol, string> = {
  Administrador: 'bg-tinta text-white',
  Recepcionista: 'bg-marea-suave text-marea-profunda',
  Medico: 'bg-estado-completada/10 text-estado-completada',
  Paciente: 'bg-papel text-pizarra border border-linea',
}

/** El último acceso se guarda en UTC sin zona; se marca como UTC para mostrarlo en hora local. */
function textoUltimoAcceso(valor: string | null) {
  if (!valor) return 'Nunca'
  const fecha = new Date(/[zZ]|[+-]\d{2}:\d{2}$/.test(valor) ? valor : `${valor}Z`)
  return formatDistanceToNow(fecha, { addSuffix: true, locale: es })
}

/** Usuarios (specs/frontend-pantallas.md, "Usuarios"): cuentas de acceso, solo para el administrador. */
export default function PaginaUsuarios() {
  const sesion = useSesion()
  const consulta = useUsuarios()
  const doctores = useListaDoctores()
  const pacientes = usePacientes()
  const eliminar = useEliminarUsuario()
  const [busqueda, setBusqueda] = useState('')
  const [rol, setRol] = useState(TODOS)
  const [enForma, setEnForma] = useState<{ usuario: Usuario | null } | null>(null)
  const [porEliminar, setPorEliminar] = useState<Usuario | null>(null)

  const nombreDoctor = (id: number) => doctores.data?.find((d) => d.id === id)?.nombreCompleto ?? `Doctor ${id}`
  const paciente = (id: number) => pacientes.data?.find((p) => p.id === id)

  const termino = normalizar(busqueda.trim())
  const filtrados = (consulta.data ?? []).filter(
    (u) =>
      (rol === TODOS || u.rol === rol) && (!termino || normalizar(`${u.nombreCompleto} ${u.email}`).includes(termino)),
  )

  return (
    <div className="grid gap-6">
      <EncabezadoPagina
        titulo="Usuarios"
        descripcion="Cuentas de acceso al sistema y a la app móvil"
        accion={
          <Button onClick={() => setEnForma({ usuario: null })}>
            <UserPlus aria-hidden="true" />
            Crear cuenta
          </Button>
        }
      />

      <div className="flex flex-wrap gap-3">
        <Buscador valor={busqueda} alCambiar={setBusqueda} etiqueta="Buscar por nombre o correo" />
        <Select value={rol} onValueChange={setRol}>
          <SelectTrigger className="h-9 w-full bg-card sm:w-48" aria-label="Filtrar por rol">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={TODOS}>Todos los roles</SelectItem>
            {ROLES.map((r) => (
              <SelectItem key={r} value={r}>
                {TEXTO_ROL[r]}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </div>

      {consulta.isPending ? (
        <EsqueletoTabla columnas={5} />
      ) : consulta.isError ? (
        <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
      ) : filtrados.length === 0 ? (
        <Vacio mensaje="Ninguna cuenta coincide con el filtro." />
      ) : (
        <div className={CONTENEDOR_TABLA}>
          <Table>
            <TableHeader>
              <TableRow className={ENCABEZADO_TABLA}>
                <TableHead className="pl-4">Usuario</TableHead>
                <TableHead>Rol</TableHead>
                <TableHead className="hidden md:table-cell">Vínculo</TableHead>
                <TableHead className="hidden sm:table-cell">Estado</TableHead>
                <TableHead className="hidden lg:table-cell">Último acceso</TableHead>
                <TableHead className="w-12">
                  <span className="sr-only">Acciones</span>
                </TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {filtrados.map((u) => {
                const esYo = u.id === sesion.usuario?.id
                const p = u.idPaciente ? paciente(u.idPaciente) : undefined
                return (
                  <TableRow key={u.id} className={cn(!u.activo && 'text-pizarra')}>
                    <TableCell className="py-3 pl-4">
                      <p className="font-medium">
                        {u.nombreCompleto}
                        {esYo ? <span className="ml-2 text-xs font-normal text-pizarra">(tú)</span> : null}
                      </p>
                      <p className="text-xs text-pizarra">{u.email}</p>
                    </TableCell>
                    <TableCell>
                      <span className={cn('inline-flex h-6 items-center rounded-full px-2.5 text-xs font-medium', COLOR_ROL[u.rol])}>
                        {TEXTO_ROL[u.rol]}
                      </span>
                    </TableCell>
                    <TableCell className="hidden md:table-cell">
                      {u.idDoctor ? (
                        nombreDoctor(u.idDoctor)
                      ) : u.idPaciente ? (
                        <Link to={`/pacientes/${u.idPaciente}`} className="text-marea hover:underline">
                          {p ? `${p.nombres} ${p.apellidos}` : `Paciente ${u.idPaciente}`}
                        </Link>
                      ) : (
                        <span className="text-pizarra">—</span>
                      )}
                    </TableCell>
                    <TableCell className="hidden sm:table-cell">
                      <InsigniaActivo activo={u.activo} textoInactivo="Inactiva" />
                    </TableCell>
                    <TableCell className="hidden text-pizarra lg:table-cell">
                      {textoUltimoAcceso(u.ultimoAcceso)}
                    </TableCell>
                    <TableCell className="pr-3">
                      <MenuFila
                        etiqueta={`Acciones de ${u.nombreCompleto}`}
                        acciones={[
                          { texto: 'Editar', icono: Pencil, alElegir: () => setEnForma({ usuario: u }) },
                          {
                            texto: 'Eliminar',
                            icono: Trash2,
                            destructiva: true,
                            deshabilitada: esYo,
                            alElegir: () => setPorEliminar(u),
                          },
                        ]}
                      />
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        </div>
      )}

      <FormularioUsuario
        abierto={enForma !== null}
        alCerrar={() => setEnForma(null)}
        usuario={enForma?.usuario ?? null}
        usuarios={consulta.data ?? []}
      />
      <Confirmar
        abierto={porEliminar !== null}
        alCambiar={(abierto) => !abierto && setPorEliminar(null)}
        titulo="¿Eliminar la cuenta?"
        descripcion={
          porEliminar
            ? `${porEliminar.nombreCompleto} ya no podrá iniciar sesión. Si solo quieres suspender el acceso, edita la cuenta y desactívala.`
            : ''
        }
        textoConfirmar="Eliminar cuenta"
        pendiente={eliminar.isPending}
        alConfirmar={() =>
          porEliminar &&
          eliminar.mutate(porEliminar.id, {
            onSuccess: () => {
              toast.success('Cuenta eliminada.')
              setPorEliminar(null)
            },
            onError: (e) => {
              toast.error(mensajeDeError(e))
              setPorEliminar(null)
            },
          })
        }
      />
    </div>
  )
}
