import { Pencil, Plus, Trash2 } from 'lucide-react'
import { useState } from 'react'
import { useSearchParams } from 'react-router'
import { toast } from 'sonner'
import { Confirmar } from '@/componentes/Confirmar'
import {
  useEliminarDoctor,
  useEliminarEspecialidad,
  useEspecialidades,
  useListaDoctores,
} from '@/componentes/doctores/consultas'
import { FormularioDoctor, FormularioEspecialidad } from '@/componentes/doctores/Formularios'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista, Vacio } from '@/componentes/EstadosVista'
import { MenuFila } from '@/componentes/MenuFila'
import { Buscador, CONTENEDOR_TABLA, ENCABEZADO_TABLA, EsqueletoTabla, InsigniaActivo } from '@/componentes/Tabla'
import { Button } from '@/components/ui/button'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { mensajeDeError } from '@/lib/api'
import { normalizar } from '@/lib/catalogos'
import { formatearCifra } from '@/lib/formato'
import type { Doctor, Especialidad } from '@/lib/tipos'
import { useSesion } from '@/sesion/SesionContext'

type Vista = 'doctores' | 'especialidades'
const TODAS = 'todas'

/**
 * Doctores y especialidades (specs/frontend-pantallas.md, "Doctores").
 * Todo el personal consulta; solo el administrador registra, edita y elimina.
 */
export default function PaginaDoctores() {
  const { usuario } = useSesion()
  const esAdmin = usuario?.rol === 'Administrador'
  const [parametros, setParametros] = useSearchParams()
  const vista: Vista = parametros.get('vista') === 'especialidades' ? 'especialidades' : 'doctores'

  const [doctorEnForma, setDoctorEnForma] = useState<{ doctor: Doctor | null } | null>(null)
  const [especialidadEnForma, setEspecialidadEnForma] = useState<{ especialidad: Especialidad | null } | null>(null)

  return (
    <div className="grid gap-6">
      <EncabezadoPagina
        titulo="Doctores"
        descripcion="Médicos de la clínica, su especialidad y horario"
        accion={
          esAdmin ? (
            vista === 'doctores' ? (
              <Button onClick={() => setDoctorEnForma({ doctor: null })}>
                <Plus aria-hidden="true" />
                Registrar doctor
              </Button>
            ) : (
              <Button onClick={() => setEspecialidadEnForma({ especialidad: null })}>
                <Plus aria-hidden="true" />
                Nueva especialidad
              </Button>
            )
          ) : null
        }
      />

      <Tabs
        value={vista}
        onValueChange={(v) => setParametros(v === 'doctores' ? {} : { vista: v }, { replace: true })}
        className="min-w-0 gap-5"
      >
        <TabsList aria-label="Catálogo">
          <TabsTrigger value="doctores" className="px-3">
            Doctores
          </TabsTrigger>
          <TabsTrigger value="especialidades" className="px-3">
            Especialidades
          </TabsTrigger>
        </TabsList>
        <TabsContent value="doctores">
          <ListaDoctores esAdmin={esAdmin} alEditar={(doctor) => setDoctorEnForma({ doctor })} />
        </TabsContent>
        <TabsContent value="especialidades">
          <ListaEspecialidades
            esAdmin={esAdmin}
            alEditar={(especialidad) => setEspecialidadEnForma({ especialidad })}
          />
        </TabsContent>
      </Tabs>

      <FormularioDoctor
        abierto={doctorEnForma !== null}
        alCerrar={() => setDoctorEnForma(null)}
        doctor={doctorEnForma?.doctor ?? null}
      />
      <FormularioEspecialidad
        abierto={especialidadEnForma !== null}
        alCerrar={() => setEspecialidadEnForma(null)}
        especialidad={especialidadEnForma?.especialidad ?? null}
      />
    </div>
  )
}

function ListaDoctores({ esAdmin, alEditar }: { esAdmin: boolean; alEditar: (doctor: Doctor) => void }) {
  const consulta = useListaDoctores()
  const especialidades = useEspecialidades()
  const eliminar = useEliminarDoctor()
  const [busqueda, setBusqueda] = useState('')
  const [especialidad, setEspecialidad] = useState(TODAS)
  const [porEliminar, setPorEliminar] = useState<Doctor | null>(null)

  const termino = normalizar(busqueda.trim())
  const filtrados = (consulta.data ?? []).filter(
    (d) =>
      (especialidad === TODAS || String(d.idEspecialidad) === especialidad) &&
      (!termino || normalizar(`${d.nombreCompleto} ${d.email} ${d.nombreEspecialidad}`).includes(termino)),
  )

  return (
    <div className="grid gap-4">
      <div className="flex flex-wrap gap-3">
        <Buscador valor={busqueda} alCambiar={setBusqueda} etiqueta="Buscar por nombre o correo" />
        <Select value={especialidad} onValueChange={setEspecialidad}>
          <SelectTrigger className="h-9 w-full bg-card sm:w-56" aria-label="Filtrar por especialidad">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={TODAS}>Todas las especialidades</SelectItem>
            {(especialidades.data ?? []).map((e) => (
              <SelectItem key={e.id} value={String(e.id)}>
                {e.nombre}
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
        <Vacio
          mensaje={consulta.data.length === 0 ? 'Todavía no hay doctores registrados.' : 'Ningún doctor coincide con el filtro.'}
        />
      ) : (
        <div className={CONTENEDOR_TABLA}>
          <Table>
            <TableHeader>
              <TableRow className={ENCABEZADO_TABLA}>
                <TableHead className="pl-4">Doctor</TableHead>
                <TableHead className="hidden sm:table-cell">Especialidad</TableHead>
                <TableHead className="hidden lg:table-cell">Horario</TableHead>
                <TableHead className="hidden md:table-cell">Teléfono</TableHead>
                <TableHead className="hidden text-right md:table-cell">Citas</TableHead>
                <TableHead>Estado</TableHead>
                {esAdmin ? (
                  <TableHead className="w-12">
                    <span className="sr-only">Acciones</span>
                  </TableHead>
                ) : null}
              </TableRow>
            </TableHeader>
            <TableBody>
              {filtrados.map((d) => (
                <TableRow key={d.id}>
                  <TableCell className="py-3 pl-4">
                    <p className="font-medium">{d.nombreCompleto}</p>
                    <p className="text-xs text-pizarra">
                      <span className="sm:hidden">{d.nombreEspecialidad} · </span>
                      {d.email}
                    </p>
                  </TableCell>
                  <TableCell className="hidden sm:table-cell">{d.nombreEspecialidad}</TableCell>
                  <TableCell className="hidden max-w-60 truncate text-pizarra lg:table-cell">
                    {d.horarioAtencion || '—'}
                  </TableCell>
                  <TableCell className="cifras hidden text-pizarra md:table-cell">{d.telefono}</TableCell>
                  <TableCell className="cifras hidden text-right md:table-cell">{formatearCifra(d.cantidadCitas)}</TableCell>
                  <TableCell>
                    <InsigniaActivo activo={d.activo} />
                  </TableCell>
                  {esAdmin ? (
                    <TableCell className="pr-3">
                      <MenuFila
                        etiqueta={`Acciones de ${d.nombreCompleto}`}
                        acciones={[
                          { texto: 'Editar', icono: Pencil, alElegir: () => alEditar(d) },
                          { texto: 'Eliminar', icono: Trash2, destructiva: true, alElegir: () => setPorEliminar(d) },
                        ]}
                      />
                    </TableCell>
                  ) : null}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}

      <Confirmar
        abierto={porEliminar !== null}
        alCambiar={(abierto) => !abierto && setPorEliminar(null)}
        titulo="¿Eliminar al doctor?"
        descripcion={
          porEliminar
            ? porEliminar.cantidadCitas > 0
              ? `${porEliminar.nombreCompleto} tiene ${porEliminar.cantidadCitas} citas registradas, así que la API no permitirá eliminarlo.`
              : `${porEliminar.nombreCompleto} dejará de aparecer en la agenda y en los catálogos.`
            : ''
        }
        textoConfirmar="Eliminar"
        pendiente={eliminar.isPending}
        alConfirmar={() =>
          porEliminar &&
          eliminar.mutate(porEliminar.id, {
            onSuccess: () => {
              toast.success('Doctor eliminado.')
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

function ListaEspecialidades({
  esAdmin,
  alEditar,
}: {
  esAdmin: boolean
  alEditar: (especialidad: Especialidad) => void
}) {
  const consulta = useEspecialidades()
  const eliminar = useEliminarEspecialidad()
  const [porEliminar, setPorEliminar] = useState<Especialidad | null>(null)

  if (consulta.isPending) return <EsqueletoTabla columnas={3} />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  if (consulta.data.length === 0) return <Vacio mensaje="Todavía no hay especialidades." />

  return (
    <>
      <ul className="grid gap-px overflow-hidden rounded-[10px] border border-linea bg-linea sm:grid-cols-2 xl:grid-cols-3">
        {consulta.data.map((e) => (
          <li key={e.id} className="flex items-start gap-3 bg-card p-5">
            <div className="min-w-0 flex-1">
              <h3 className="font-heading font-semibold">{e.nombre}</h3>
              <p className="mt-1 line-clamp-2 text-pizarra">{e.descripcion || 'Sin descripción'}</p>
              <p className="mt-3 text-xs text-pizarra">
                <span className="cifras font-medium text-tinta">{e.cantidadDoctores}</span>{' '}
                {e.cantidadDoctores === 1 ? 'doctor' : 'doctores'}
              </p>
            </div>
            {esAdmin ? (
              <MenuFila
                etiqueta={`Acciones de ${e.nombre}`}
                acciones={[
                  { texto: 'Editar', icono: Pencil, alElegir: () => alEditar(e) },
                  { texto: 'Eliminar', icono: Trash2, destructiva: true, alElegir: () => setPorEliminar(e) },
                ]}
              />
            ) : null}
          </li>
        ))}
      </ul>
      <Confirmar
        abierto={porEliminar !== null}
        alCambiar={(abierto) => !abierto && setPorEliminar(null)}
        titulo="¿Eliminar la especialidad?"
        descripcion={
          porEliminar
            ? porEliminar.cantidadDoctores > 0
              ? `${porEliminar.nombre} tiene doctores asignados; cámbialos de especialidad antes de eliminarla.`
              : `${porEliminar.nombre} se eliminará del catálogo.`
            : ''
        }
        textoConfirmar="Eliminar"
        pendiente={eliminar.isPending}
        alConfirmar={() =>
          porEliminar &&
          eliminar.mutate(porEliminar.id, {
            onSuccess: () => {
              toast.success('Especialidad eliminada.')
              setPorEliminar(null)
            },
            onError: (err) => {
              toast.error(mensajeDeError(err))
              setPorEliminar(null)
            },
          })
        }
      />
    </>
  )
}
