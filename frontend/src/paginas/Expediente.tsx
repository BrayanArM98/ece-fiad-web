import { format } from 'date-fns'
import { es } from 'date-fns/locale'
import { ArrowLeft, CalendarPlus, FilePlus2, Pencil, Plus, TriangleAlert, UserX } from 'lucide-react'
import { useState, type ReactNode } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router'
import { toast } from 'sonner'
import { FormularioCita } from '@/componentes/agenda/FormularioCita'
import { ContenidoCarpeta, ListaCarpeta, PestanaCarpeta } from '@/componentes/Carpeta'
import { Confirmar } from '@/componentes/Confirmar'
import { ErrorVista, Vacio } from '@/componentes/EstadosVista'
import { InsigniaEstado } from '@/componentes/InsigniaEstado'
import { FormularioEvolucion } from '@/componentes/pacientes/FormularioEvolucion'
import { FormularioHistoria } from '@/componentes/pacientes/FormularioHistoria'
import { FormularioPaciente } from '@/componentes/pacientes/FormularioPaciente'
import {
  useCitasPaciente,
  useDarDeBajaPaciente,
  useHistorial,
  usePaciente,
} from '@/componentes/pacientes/consultas'
import { CONTENEDOR_TABLA, ENCABEZADO_TABLA, InsigniaActivo } from '@/componentes/Tabla'
import { Button } from '@/components/ui/button'
import { Skeleton } from '@/components/ui/skeleton'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Tabs } from '@/components/ui/tabs'
import { mensajeDeError } from '@/lib/api'
import { numeroExpediente, TEXTO_DOCUMENTO, TEXTO_GENERO, TEXTO_GRUPO, textoEdad } from '@/lib/catalogos'
import { aFecha, formatearFechaCorta, formatearHora } from '@/lib/formato'
import type { Evolucion, HistorialPaciente, Paciente } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { useSesion } from '@/sesion/SesionContext'

type Pestana = 'datos' | 'historia' | 'evoluciones' | 'citas'

const fechaLarga = (valor: string) => format(aFecha(valor), "d 'de' MMMM 'de' yyyy", { locale: es })

/**
 * Expediente del paciente (specs/frontend-pantallas.md, "Expediente del paciente").
 * La recepción solo ve los datos y las citas: la historia clínica es información médica.
 */
export default function PaginaExpediente() {
  const { id } = useParams()
  const idPaciente = Number(id) || 0
  const { usuario } = useSesion()
  const rol = usuario?.rol
  const verClinico = rol === 'Administrador' || rol === 'Medico'
  const puedeEditarDatos = rol === 'Administrador' || rol === 'Recepcionista'

  const paciente = usePaciente(idPaciente)
  const historial = useHistorial(idPaciente, verClinico)

  const [parametros, setParametros] = useSearchParams()
  const pestanas: Pestana[] = verClinico ? ['datos', 'historia', 'evoluciones', 'citas'] : ['datos', 'citas']
  const solicitada = parametros.get('pestana') as Pestana | null
  const pestana: Pestana = solicitada && pestanas.includes(solicitada) ? solicitada : 'datos'

  if (paciente.isPending) return <EsqueletoExpediente />
  if (paciente.isError) {
    return (
      <div className="grid gap-6">
        <VolverAPacientes />
        <ErrorVista error={paciente.error} alReintentar={() => paciente.refetch()} />
      </div>
    )
  }

  const p = paciente.data

  return (
    <div className="grid gap-6">
      <VolverAPacientes />
      <EncabezadoExpediente paciente={p} historial={verClinico ? historial.data : undefined} />

      <Tabs
        value={pestana}
        onValueChange={(valor) => {
          const siguientes = new URLSearchParams(parametros)
          if (valor === 'datos') siguientes.delete('pestana')
          else siguientes.set('pestana', valor)
          setParametros(siguientes, { replace: true })
        }}
        className="min-w-0 gap-0"
      >
        <ListaCarpeta aria-label="Secciones del expediente">
          <PestanaCarpeta value="datos">Datos</PestanaCarpeta>
          {verClinico ? (
            <>
              <PestanaCarpeta value="historia">Historia clínica</PestanaCarpeta>
              <PestanaCarpeta value="evoluciones">
                Evoluciones
                {historial.data?.tieneHistoria ? (
                  <span className="cifras rounded-full bg-marea-suave px-1.5 text-xs text-marea-profunda">
                    {historial.data.totalEvoluciones}
                  </span>
                ) : null}
              </PestanaCarpeta>
            </>
          ) : null}
          <PestanaCarpeta value="citas">Citas</PestanaCarpeta>
        </ListaCarpeta>

        <ContenidoCarpeta value="datos" className="mt-0">
          <PestanaDatos paciente={p} puedeEditar={puedeEditarDatos} />
        </ContenidoCarpeta>
        {verClinico ? (
          <>
            <ContenidoCarpeta value="historia" className="mt-0">
              <PestanaHistoria idPaciente={p.id} consulta={historial} />
            </ContenidoCarpeta>
            <ContenidoCarpeta value="evoluciones" className="mt-0">
              <PestanaEvoluciones idPaciente={p.id} consulta={historial} />
            </ContenidoCarpeta>
          </>
        ) : null}
        <ContenidoCarpeta value="citas" className="mt-0">
          <PestanaCitas paciente={p} puedeAgendar={puedeEditarDatos} />
        </ContenidoCarpeta>
      </Tabs>
    </div>
  )
}

function VolverAPacientes() {
  return (
    <Link
      to="/pacientes"
      className="inline-flex w-fit items-center gap-1.5 text-pizarra hover:text-tinta focus-visible:text-tinta"
    >
      <ArrowLeft className="size-4" aria-hidden="true" />
      Pacientes
    </Link>
  )
}

// ============================================
// Encabezado con la banda de alergias
// ============================================

function EncabezadoExpediente({ paciente: p, historial }: { paciente: Paciente; historial?: HistorialPaciente }) {
  const iniciales = `${p.nombres.charAt(0)}${p.apellidos.charAt(0)}`.toUpperCase()
  const alergias = historial?.historia?.alergias.trim()

  return (
    <header className="grid gap-4">
      <div className="flex flex-wrap items-center gap-4">
        <div
          className="grid size-14 shrink-0 place-items-center rounded-full bg-marea-suave font-heading text-lg font-semibold text-marea-profunda"
          aria-hidden="true"
        >
          {iniciales}
        </div>
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-x-3 gap-y-1">
            <h1 className="text-[1.75rem] leading-tight font-semibold tracking-tight">
              {p.nombres} {p.apellidos}
            </h1>
            {!p.activo ? <InsigniaActivo activo={false} /> : null}
          </div>
          <p className="mt-1 flex flex-wrap gap-x-2 text-pizarra">
            <span>{textoEdad(p.fechaNacimiento)}</span>
            <Separador />
            <span>{TEXTO_GENERO[p.genero]}</span>
            <Separador />
            <span>
              Grupo <strong className="font-semibold text-tinta">{TEXTO_GRUPO[p.grupoSanguineo]}</strong>
            </span>
            <Separador />
            <span className="cifras">{numeroExpediente(p.id)}</span>
          </p>
        </div>
      </div>

      {historial ? (
        historial.tieneHistoria ? (
          alergias ? (
            <div role="note" className="flex items-start gap-3 rounded-md border border-coral/30 bg-coral-suave px-4 py-3 text-coral">
              <TriangleAlert className="mt-0.5 size-4 shrink-0" aria-hidden="true" />
              <p>
                <strong className="font-semibold">Alergias:</strong> <span className="text-tinta">{alergias}</span>
              </p>
            </div>
          ) : (
            <p className="rounded-md border border-linea bg-card px-4 py-2.5 text-pizarra">Sin alergias conocidas.</p>
          )
        ) : (
          <p className="rounded-md border border-dashed border-linea px-4 py-2.5 text-pizarra">
            Sin historia clínica: las alergias se registran al abrirla.
          </p>
        )
      ) : null}
    </header>
  )
}

const Separador = () => (
  <span aria-hidden="true" className="text-linea">
    ·
  </span>
)

// ============================================
// Datos
// ============================================

function PestanaDatos({ paciente: p, puedeEditar }: { paciente: Paciente; puedeEditar: boolean }) {
  const navegar = useNavigate()
  const [editando, setEditando] = useState(false)
  const [confirmarBaja, setConfirmarBaja] = useState(false)
  const darDeBaja = useDarDeBajaPaciente()

  return (
    <div className="grid gap-6">
      <dl className="grid gap-x-8 gap-y-5 sm:grid-cols-2 lg:grid-cols-3">
        <Dato titulo="Nombres">{p.nombres}</Dato>
        <Dato titulo="Apellidos">{p.apellidos}</Dato>
        <Dato titulo="Documento">
          {TEXTO_DOCUMENTO[p.tipoDocumento]} <span className="cifras">{p.numeroDocumento}</span>
        </Dato>
        <Dato titulo="Fecha de nacimiento">
          {fechaLarga(p.fechaNacimiento)} <span className="text-pizarra">({textoEdad(p.fechaNacimiento)})</span>
        </Dato>
        <Dato titulo="Género">{TEXTO_GENERO[p.genero]}</Dato>
        <Dato titulo="Grupo sanguíneo">{TEXTO_GRUPO[p.grupoSanguineo]}</Dato>
        <Dato titulo="Teléfono">{p.telefono ? <span className="cifras">{p.telefono}</span> : <SinDato />}</Dato>
        <Dato titulo="Correo">{p.email || <SinDato />}</Dato>
        <Dato titulo="Estado">
          <InsigniaActivo activo={p.activo} />
        </Dato>
        <Dato titulo="Dirección" className="sm:col-span-2 lg:col-span-3">
          {p.direccion || <SinDato />}
        </Dato>
      </dl>

      {puedeEditar ? (
        <div className="flex flex-wrap justify-between gap-3 border-t border-linea pt-5">
          <Button variant="destructive" onClick={() => setConfirmarBaja(true)}>
            <UserX aria-hidden="true" />
            Dar de baja
          </Button>
          <Button variant="outline" onClick={() => setEditando(true)}>
            <Pencil aria-hidden="true" />
            Editar datos
          </Button>
        </div>
      ) : null}

      <FormularioPaciente abierto={editando} alCerrar={() => setEditando(false)} paciente={p} />
      <Confirmar
        abierto={confirmarBaja}
        alCambiar={setConfirmarBaja}
        titulo="¿Dar de baja al paciente?"
        descripcion={`${p.nombres} ${p.apellidos} dejará de aparecer en la lista de pacientes y no se le podrán agendar citas. Su expediente se conserva.`}
        textoConfirmar="Dar de baja"
        pendiente={darDeBaja.isPending}
        alConfirmar={() =>
          darDeBaja.mutate(p.id, {
            onSuccess: () => {
              toast.success('Paciente dado de baja.')
              navegar('/pacientes')
            },
            onError: (e) => toast.error(mensajeDeError(e)),
          })
        }
      />
    </div>
  )
}

function Dato({ titulo, className, children }: { titulo: string; className?: string; children: ReactNode }) {
  return (
    <div className={cn('grid content-start gap-1', className)}>
      <dt className="text-xs font-medium text-pizarra">{titulo}</dt>
      <dd className="text-tinta">{children}</dd>
    </div>
  )
}

const SinDato = () => <span className="text-pizarra">Sin registrar</span>

// ============================================
// Historia clínica
// ============================================

type ConsultaHistorial = ReturnType<typeof useHistorial>

function PestanaHistoria({ idPaciente, consulta }: { idPaciente: number; consulta: ConsultaHistorial }) {
  const [editando, setEditando] = useState(false)

  if (consulta.isPending) return <EsqueletoBloques />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />

  const historia = consulta.data.historia

  return (
    <>
      {!consulta.data.tieneHistoria || !historia ? (
        <Vacio
          mensaje="Este paciente aún no tiene historia clínica."
          accion={
            <Button onClick={() => setEditando(true)}>
              <FilePlus2 aria-hidden="true" />
              Abrir historia clínica
            </Button>
          }
        />
      ) : (
        <div className="grid gap-6">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <p className="text-pizarra">Abierta el {fechaLarga(historia.fechaApertura)}</p>
            <Button variant="outline" onClick={() => setEditando(true)}>
              <Pencil aria-hidden="true" />
              Editar
            </Button>
          </div>
          <div className="grid gap-px overflow-hidden rounded-[10px] border border-linea bg-linea md:grid-cols-3">
            <BloqueHistoria titulo="Alergias" texto={historia.alergias} vacio="Sin alergias conocidas" alerta />
            <BloqueHistoria titulo="Antecedentes personales" texto={historia.antecedentesPersonales} />
            <BloqueHistoria titulo="Antecedentes familiares" texto={historia.antecedentesFamiliares} />
          </div>
        </div>
      )}
      <FormularioHistoria
        abierto={editando}
        alCerrar={() => setEditando(false)}
        idPaciente={idPaciente}
        historia={historia}
      />
    </>
  )
}

function BloqueHistoria({
  titulo,
  texto,
  vacio = 'Sin registro',
  alerta,
}: {
  titulo: string
  texto: string
  vacio?: string
  alerta?: boolean
}) {
  const conTexto = texto.trim().length > 0
  return (
    <section className="grid content-start gap-2 bg-card p-5">
      <h3 className={cn('font-heading font-semibold', alerta && conTexto && 'text-coral')}>{titulo}</h3>
      <p className={cn('whitespace-pre-line', conTexto ? 'text-tinta' : 'text-pizarra')}>{conTexto ? texto : vacio}</p>
    </section>
  )
}

// ============================================
// Evoluciones
// ============================================

function PestanaEvoluciones({ idPaciente, consulta }: { idPaciente: number; consulta: ConsultaHistorial }) {
  const { usuario } = useSesion()
  const [formulario, setFormulario] = useState<{ evolucion: Evolucion | null } | null>(null)

  if (consulta.isPending) return <EsqueletoBloques />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />

  const { historia, evoluciones } = consulta.data
  if (!historia) {
    return <Vacio mensaje="Abre la historia clínica del paciente para registrar evoluciones." />
  }

  const puedeEditar = (e: Evolucion) =>
    usuario?.rol === 'Administrador' || (usuario?.rol === 'Medico' && usuario.idDoctor === e.idDoctor)

  return (
    <div className="grid gap-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <p className="text-pizarra">
          {evoluciones.length === 0
            ? 'Sin evoluciones registradas'
            : evoluciones.length === 1
              ? '1 evolución, la más reciente primero'
              : `${evoluciones.length} evoluciones, la más reciente primero`}
        </p>
        <Button onClick={() => setFormulario({ evolucion: null })}>
          <Plus aria-hidden="true" />
          Registrar evolución
        </Button>
      </div>

      {evoluciones.length === 0 ? (
        <Vacio mensaje="Cuando el paciente pase a consulta, registra aquí el diagnóstico y el tratamiento." />
      ) : (
        <ol className="grid">
          {evoluciones.map((e, i) => (
            <li key={e.id} className="relative grid grid-cols-[4.5rem_1fr] gap-4 pb-6 last:pb-0 sm:grid-cols-[6rem_1fr]">
              {/* Línea de tiempo */}
              {i < evoluciones.length - 1 ? (
                <span className="absolute top-4 bottom-0 left-[5rem] w-px bg-linea sm:left-[6.5rem]" aria-hidden="true" />
              ) : null}
              <time dateTime={e.fecha} className="grid content-start text-right">
                <span className="cifras font-heading text-lg leading-tight font-semibold text-tinta">
                  {format(aFecha(e.fecha), 'd MMM', { locale: es }).replace('.', '')}
                </span>
                <span className="cifras text-xs text-pizarra">{format(aFecha(e.fecha), 'yyyy')}</span>
              </time>
              <article className="relative grid gap-3 rounded-md border border-linea p-4">
                <span
                  className="absolute top-3.5 -left-[calc(0.5rem+4.5px)] size-2 rounded-full bg-marea ring-4 ring-card"
                  aria-hidden="true"
                />
                <header className="flex flex-wrap items-start justify-between gap-2">
                  <div>
                    <h3 className="font-heading font-semibold">{e.diagnostico}</h3>
                    <p className="text-xs text-pizarra">
                      {e.nombreDoctor} · {e.nombreEspecialidad} · {formatearHora(e.fecha)}
                    </p>
                  </div>
                  {puedeEditar(e) ? (
                    <Button variant="ghost" size="sm" onClick={() => setFormulario({ evolucion: e })}>
                      <Pencil aria-hidden="true" />
                      Editar
                    </Button>
                  ) : null}
                </header>
                <dl className="grid gap-3 sm:grid-cols-2">
                  <div className="grid content-start gap-0.5">
                    <dt className="text-xs font-medium text-pizarra">Tratamiento</dt>
                    <dd className="whitespace-pre-line">{e.tratamiento}</dd>
                  </div>
                  {e.notas ? (
                    <div className="grid content-start gap-0.5">
                      <dt className="text-xs font-medium text-pizarra">Notas</dt>
                      <dd className="whitespace-pre-line">{e.notas}</dd>
                    </div>
                  ) : null}
                </dl>
              </article>
            </li>
          ))}
        </ol>
      )}

      <FormularioEvolucion
        abierto={formulario !== null}
        alCerrar={() => setFormulario(null)}
        idPaciente={idPaciente}
        idHistoria={historia.id}
        evolucion={formulario?.evolucion ?? null}
      />
    </div>
  )
}

// ============================================
// Citas
// ============================================

function PestanaCitas({ paciente, puedeAgendar }: { paciente: Paciente; puedeAgendar: boolean }) {
  const consulta = useCitasPaciente(paciente.id)
  const [agendando, setAgendando] = useState(false)
  const ahora = new Date()
  const proximas = (consulta.data ?? []).filter((c) => aFecha(c.fechaHora) >= ahora).length

  return (
    <div className="grid gap-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <p className="text-pizarra">
          {consulta.data
            ? `${consulta.data.length} citas en total${proximas ? `, ${proximas} próximas` : ''}`
            : 'Citas del paciente'}
        </p>
        {puedeAgendar && paciente.activo ? (
          <Button onClick={() => setAgendando(true)}>
            <CalendarPlus aria-hidden="true" />
            Agendar cita
          </Button>
        ) : null}
      </div>

      {consulta.isPending ? (
        <EsqueletoBloques />
      ) : consulta.isError ? (
        <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
      ) : consulta.data.length === 0 ? (
        <Vacio mensaje="Este paciente no tiene citas." />
      ) : (
        <div className={CONTENEDOR_TABLA}>
          <Table>
            <TableHeader>
              <TableRow className={ENCABEZADO_TABLA}>
                <TableHead className="pl-4">Fecha</TableHead>
                <TableHead className="hidden sm:table-cell">Doctor</TableHead>
                <TableHead className="hidden md:table-cell">Motivo</TableHead>
                <TableHead className="pr-4">Estado</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {consulta.data.map((c) => (
                <TableRow key={c.id} className={cn(aFecha(c.fechaHora) >= ahora && 'bg-marea-suave/30')}>
                  <TableCell className="py-3 pl-4">
                    <span className="cifras font-medium">{formatearFechaCorta(c.fechaHora)}</span>
                    <span className="cifras ml-2 text-pizarra">{formatearHora(c.fechaHora)}</span>
                    <p className="text-xs text-pizarra sm:hidden">{c.nombreDoctor}</p>
                  </TableCell>
                  <TableCell className="hidden sm:table-cell">{c.nombreDoctor}</TableCell>
                  <TableCell className="hidden max-w-72 truncate text-pizarra md:table-cell">
                    {c.motivo || '—'}
                  </TableCell>
                  <TableCell className="pr-4">
                    <InsigniaEstado estado={c.estado} />
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}

      <FormularioCita
        abierto={agendando}
        alCerrar={() => setAgendando(false)}
        iniciales={{ idPaciente: paciente.id }}
      />
    </div>
  )
}

// ============================================
// Esqueletos
// ============================================

function EsqueletoBloques() {
  return (
    <div className="grid gap-3">
      <Skeleton className="h-5 w-48" />
      <Skeleton className="h-24" />
      <Skeleton className="h-24" />
    </div>
  )
}

function EsqueletoExpediente() {
  return (
    <div className="grid gap-6">
      <Skeleton className="h-5 w-24" />
      <div className="flex items-center gap-4">
        <Skeleton className="size-14 rounded-full" />
        <div className="grid gap-2">
          <Skeleton className="h-8 w-64" />
          <Skeleton className="h-4 w-80" />
        </div>
      </div>
      <Skeleton className="h-11" />
      <Skeleton className="h-64" />
    </div>
  )
}
