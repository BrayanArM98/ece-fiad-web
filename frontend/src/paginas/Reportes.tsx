import { useQuery } from '@tanstack/react-query'
import { addDays, endOfMonth, format, startOfMonth, startOfYear, subMonths } from 'date-fns'
import { es } from 'date-fns/locale'
import { FileDown, FileSpreadsheet } from 'lucide-react'
import { useState, type ReactNode } from 'react'
import { useSearchParams } from 'react-router'
import { toast } from 'sonner'
import { useDoctores } from '@/componentes/agenda/consultas'
import { useEspecialidades } from '@/componentes/doctores/consultas'
import { EncabezadoPagina } from '@/componentes/EncabezadoPagina'
import { ErrorVista } from '@/componentes/EstadosVista'
import { EsqueletoIndicadores, RejillaIndicadores } from '@/componentes/Indicadores'
import { BarrasConteo, colorEstado, GraficaSerieCitas } from '@/componentes/reportes/Graficas'
import { CONTENEDOR_TABLA, ENCABEZADO_TABLA } from '@/componentes/Tabla'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select'
import { Skeleton } from '@/components/ui/skeleton'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import { Tabs, TabsList, TabsTrigger } from '@/components/ui/tabs'
import { descargar, mensajeDeError, pedir } from '@/lib/api'
import { aFecha, aFechaApi, formatearCifra } from '@/lib/formato'
import type {
  Periodo,
  ReporteCitas,
  ReporteDiagnosticos,
  ReportePacientes,
  ReporteProductividad,
  ResumenGeneral,
  Rol,
} from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { useSesion } from '@/sesion/SesionContext'

type TipoReporte = 'resumen' | 'citas' | 'productividad' | 'pacientes' | 'diagnosticos'
type Preset = 'mes' | 'mesAnterior' | 'ultimos30' | 'ultimos90' | 'anio' | 'personalizado'

const REPORTES: { tipo: TipoReporte; titulo: string; roles: Rol[] }[] = [
  { tipo: 'resumen', titulo: 'Resumen', roles: ['Administrador', 'Medico'] },
  { tipo: 'citas', titulo: 'Citas', roles: ['Administrador', 'Recepcionista', 'Medico'] },
  { tipo: 'productividad', titulo: 'Productividad', roles: ['Administrador', 'Medico'] },
  { tipo: 'pacientes', titulo: 'Pacientes', roles: ['Administrador'] },
  { tipo: 'diagnosticos', titulo: 'Diagnósticos', roles: ['Administrador', 'Medico'] },
]

const PRESETS: { valor: Preset; texto: string }[] = [
  { valor: 'mes', texto: 'Este mes' },
  { valor: 'mesAnterior', texto: 'Mes anterior' },
  { valor: 'ultimos30', texto: 'Últimos 30 días' },
  { valor: 'ultimos90', texto: 'Últimos 90 días' },
  { valor: 'anio', texto: 'Este año' },
  { valor: 'personalizado', texto: 'Personalizado' },
]

const TODOS = 'todos'

function rangoDe(preset: Preset): { desde: string; hasta: string } {
  const hoy = new Date()
  switch (preset) {
    case 'mesAnterior': {
      const mes = subMonths(hoy, 1)
      return { desde: aFechaApi(startOfMonth(mes)), hasta: aFechaApi(endOfMonth(mes)) }
    }
    case 'ultimos30':
      return { desde: aFechaApi(addDays(hoy, -29)), hasta: aFechaApi(hoy) }
    case 'ultimos90':
      return { desde: aFechaApi(addDays(hoy, -89)), hasta: aFechaApi(hoy) }
    case 'anio':
      return { desde: aFechaApi(startOfYear(hoy)), hasta: aFechaApi(hoy) }
    default:
      return { desde: aFechaApi(startOfMonth(hoy)), hasta: aFechaApi(endOfMonth(hoy)) }
  }
}

function textoPeriodo(periodo: Periodo) {
  const [desde, hasta] = [aFecha(periodo.desde), aFecha(periodo.hasta)]
  const mismoAnio = desde.getFullYear() === hasta.getFullYear()
  const inicio = format(desde, mismoAnio ? "d 'de' MMMM" : "d 'de' MMMM 'de' yyyy", { locale: es })
  const fin = format(hasta, "d 'de' MMMM 'de' yyyy", { locale: es })
  return `Del ${inicio} al ${fin} · ${periodo.dias} ${periodo.dias === 1 ? 'día' : 'días'}`
}

/**
 * Reportes (specs/reportes.md y specs/frontend-pantallas.md, "Reportes"): filtros comunes arriba,
 * una pestaña por reporte según el rol y descarga en PDF o Excel con los mismos filtros.
 * Para un médico la API siempre devuelve solo sus datos, por eso no ve el filtro de doctor.
 */
export default function PaginaReportes() {
  const { usuario } = useSesion()
  const rol = usuario?.rol
  const disponibles = REPORTES.filter((r) => rol && r.roles.includes(rol))
  const [parametros, setParametros] = useSearchParams()
  const pedido = parametros.get('reporte') as TipoReporte | null
  const reporte: TipoReporte =
    pedido && disponibles.some((r) => r.tipo === pedido) ? pedido : (disponibles[0]?.tipo ?? 'citas')

  const [preset, setPreset] = useState<Preset>('mes')
  const [personalizado, setPersonalizado] = useState(() => rangoDe('mes'))
  const [idDoctor, setIdDoctor] = useState(TODOS)
  const [idEspecialidad, setIdEspecialidad] = useState(TODOS)
  const [descargando, setDescargando] = useState<'Pdf' | 'Excel' | null>(null)

  const doctores = useDoctores()
  const especialidades = useEspecialidades()
  const esMedico = rol === 'Medico'

  const rango = preset === 'personalizado' ? personalizado : rangoDe(preset)
  const rangoValido = !!rango.desde && !!rango.hasta && rango.desde <= rango.hasta
  const filtro = {
    desde: rango.desde,
    hasta: rango.hasta,
    idDoctor: !esMedico && idDoctor !== TODOS ? Number(idDoctor) : undefined,
    idEspecialidad: idEspecialidad !== TODOS ? Number(idEspecialidad) : undefined,
  }

  const exportar = async (formato: 'Pdf' | 'Excel') => {
    setDescargando(formato)
    try {
      await descargar(`/api/Reportes/${reporte}/exportar`, { ...filtro, formato })
    } catch (error) {
      toast.error(mensajeDeError(error))
    } finally {
      setDescargando(null)
    }
  }

  return (
    <div className="grid gap-6">
      <EncabezadoPagina
        titulo="Reportes"
        descripcion={esMedico ? 'Tus indicadores, citas y diagnósticos' : 'Indicadores de la clínica por periodo'}
        accion={
          <div className="flex gap-2">
            <Button variant="outline" disabled={!rangoValido || descargando !== null} onClick={() => exportar('Pdf')}>
              <FileDown aria-hidden="true" />
              {descargando === 'Pdf' ? 'Generando…' : 'Descargar PDF'}
            </Button>
            <Button variant="outline" disabled={!rangoValido || descargando !== null} onClick={() => exportar('Excel')}>
              <FileSpreadsheet aria-hidden="true" />
              {descargando === 'Excel' ? 'Generando…' : 'Descargar Excel'}
            </Button>
          </div>
        }
      />

      {/* Filtros comunes */}
      <section
        aria-label="Filtros"
        className="flex flex-wrap items-end gap-4 rounded-[10px] border border-linea bg-card p-4"
      >
        <Filtro etiqueta="Periodo" id="filtro-periodo">
          <Select value={preset} onValueChange={(v) => setPreset(v as Preset)}>
            <SelectTrigger id="filtro-periodo" className="h-9 w-full sm:w-44">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              {PRESETS.map((p) => (
                <SelectItem key={p.valor} value={p.valor}>
                  {p.texto}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </Filtro>
        {preset === 'personalizado' ? (
          <>
            <Filtro etiqueta="Desde" id="filtro-desde">
              <Input
                id="filtro-desde"
                type="date"
                value={personalizado.desde}
                max={personalizado.hasta}
                onChange={(e) => setPersonalizado((r) => ({ ...r, desde: e.target.value }))}
                className="h-9 sm:w-40"
              />
            </Filtro>
            <Filtro etiqueta="Hasta" id="filtro-hasta">
              <Input
                id="filtro-hasta"
                type="date"
                value={personalizado.hasta}
                min={personalizado.desde}
                onChange={(e) => setPersonalizado((r) => ({ ...r, hasta: e.target.value }))}
                className="h-9 sm:w-40"
              />
            </Filtro>
          </>
        ) : null}
        {!esMedico ? (
          <Filtro etiqueta="Doctor" id="filtro-doctor">
            <Select value={idDoctor} onValueChange={setIdDoctor}>
              <SelectTrigger id="filtro-doctor" className="h-9 w-full sm:w-56">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={TODOS}>Todos los doctores</SelectItem>
                {(doctores.data ?? []).map((d) => (
                  <SelectItem key={d.id} value={String(d.id)}>
                    {d.nombreCompleto}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Filtro>
        ) : null}
        {!esMedico ? (
          <Filtro etiqueta="Especialidad" id="filtro-especialidad">
            <Select value={idEspecialidad} onValueChange={setIdEspecialidad}>
              <SelectTrigger id="filtro-especialidad" className="h-9 w-full sm:w-52">
                <SelectValue />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value={TODOS}>Todas las especialidades</SelectItem>
                {(especialidades.data ?? []).map((e) => (
                  <SelectItem key={e.id} value={String(e.id)}>
                    {e.nombre}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </Filtro>
        ) : null}
        {!rangoValido ? <p className="text-xs text-coral">La fecha inicial debe ser anterior a la final.</p> : null}
      </section>

      {disponibles.length > 1 ? (
        <Tabs
          className="min-w-0"
          value={reporte}
          onValueChange={(v) => setParametros(v === disponibles[0].tipo ? {} : { reporte: v }, { replace: true })}
        >
          <TabsList aria-label="Reporte" className="max-w-full overflow-x-auto">
            {disponibles.map((r) => (
              <TabsTrigger key={r.tipo} value={r.tipo} className="px-3">
                {r.titulo}
              </TabsTrigger>
            ))}
          </TabsList>
        </Tabs>
      ) : null}

      {rangoValido ? <ContenidoReporte tipo={reporte} filtro={filtro} esMedico={esMedico} /> : null}
    </div>
  )
}

function Filtro({ etiqueta, id, children }: { etiqueta: string; id: string; children: ReactNode }) {
  return (
    <div className="grid w-full gap-1.5 sm:w-auto">
      <Label htmlFor={id} className="text-xs font-medium text-pizarra">
        {etiqueta}
      </Label>
      {children}
    </div>
  )
}

type FiltroReporte = { desde: string; hasta: string; idDoctor?: number; idEspecialidad?: number }

function useReporte<T>(tipo: TipoReporte, filtro: FiltroReporte, habilitada: boolean) {
  return useQuery({
    queryKey: ['reportes', tipo, filtro],
    queryFn: () => pedir<T>(`/api/Reportes/${tipo}`, { parametros: { ...filtro } }),
    enabled: habilitada,
    placeholderData: (anterior) => anterior,
  })
}

function ContenidoReporte({ tipo, filtro, esMedico }: { tipo: TipoReporte; filtro: FiltroReporte; esMedico: boolean }) {
  switch (tipo) {
    case 'resumen':
      return <ReporteResumen filtro={filtro} />
    case 'citas':
      return <ReporteDeCitas filtro={filtro} esMedico={esMedico} />
    case 'productividad':
      return <ReporteDeProductividad filtro={filtro} />
    case 'pacientes':
      return <ReporteDePacientes filtro={filtro} />
    case 'diagnosticos':
      return <ReporteDeDiagnosticos filtro={filtro} />
  }
}

// ============================================
// Piezas comunes
// ============================================

function Tarjeta({ titulo, descripcion, className, children }: { titulo: string; descripcion?: string; className?: string; children: ReactNode }) {
  return (
    <section className={cn('grid content-start gap-4 rounded-[10px] border border-linea bg-card p-5', className)}>
      <div>
        <h2 className="font-heading text-base font-semibold">{titulo}</h2>
        {descripcion ? <p className="text-xs text-pizarra">{descripcion}</p> : null}
      </div>
      {children}
    </section>
  )
}

function Cifra({ titulo, valor, detalle }: { titulo: string; valor: string; detalle?: string }) {
  return (
    <div className="grid content-start gap-1 bg-card p-5">
      <dt className="text-pizarra">{titulo}</dt>
      <dd className="font-heading text-[2rem] leading-tight font-semibold text-tinta">{valor}</dd>
      {detalle ? <dd className="text-xs text-pizarra">{detalle}</dd> : null}
    </div>
  )
}

function LineaPeriodo({ periodo, cargando }: { periodo?: Periodo; cargando: boolean }) {
  return (
    <p className={cn('-mt-2 text-pizarra transition-opacity', cargando && 'opacity-50')} aria-live="polite">
      {periodo ? textoPeriodo(periodo) : ' '}
    </p>
  )
}

function Esqueleto() {
  return (
    <div className="grid gap-4 lg:grid-cols-3">
      <Skeleton className="h-80 lg:col-span-2" />
      <Skeleton className="h-80" />
    </div>
  )
}

// ============================================
// 1. Resumen
// ============================================

function ReporteResumen({ filtro }: { filtro: FiltroReporte }) {
  const consulta = useReporte<ResumenGeneral>('resumen', filtro, true)
  if (consulta.isPending) return <EsqueletoIndicadores />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  const { periodo, periodoAnterior, indicadores } = consulta.data
  return (
    <div className="grid gap-4">
      <LineaPeriodo periodo={periodo} cargando={consulta.isFetching} />
      <RejillaIndicadores indicadores={indicadores} comparadoCon="el periodo anterior" />
      <p className="text-xs text-pizarra">Se compara con {textoPeriodo(periodoAnterior).replace('Del', 'el periodo del')}.</p>
    </div>
  )
}

// ============================================
// 2. Citas
// ============================================

function ReporteDeCitas({ filtro, esMedico }: { filtro: FiltroReporte; esMedico: boolean }) {
  const consulta = useReporte<ReporteCitas>('citas', filtro, true)
  if (consulta.isPending) return <Esqueleto />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  const r = consulta.data

  return (
    <div className="grid gap-4">
      <LineaPeriodo periodo={r.periodo} cargando={consulta.isFetching} />
      <div className="grid gap-4 lg:grid-cols-3">
        <Tarjeta
          titulo="Citas en el tiempo"
          descripcion={`${formatearCifra(r.total)} citas, agrupadas por ${r.agrupacion === 'dia' ? 'día' : 'semana'}`}
          className="lg:col-span-2"
        >
          {r.total === 0 ? (
            <p className="py-16 text-center text-pizarra">No hay citas en el periodo.</p>
          ) : (
            <GraficaSerieCitas serie={r.serie} agrupacion={r.agrupacion} />
          )}
        </Tarjeta>
        <Tarjeta titulo="Por estado" descripcion="Resultado de las citas del periodo">
          <BarrasConteo conteos={r.porEstado} claseBarra={colorEstado} />
        </Tarjeta>
      </div>
      <div className={cn('grid gap-4', !esMedico && 'md:grid-cols-2')}>
        {!esMedico ? (
          <Tarjeta titulo="Por doctor">
            <BarrasConteo conteos={r.porDoctor} />
          </Tarjeta>
        ) : null}
        <Tarjeta titulo="Por especialidad">
          <BarrasConteo conteos={r.porEspecialidad} />
        </Tarjeta>
      </div>
    </div>
  )
}

// ============================================
// 3. Productividad
// ============================================

function ReporteDeProductividad({ filtro }: { filtro: FiltroReporte }) {
  const consulta = useReporte<ReporteProductividad>('productividad', filtro, true)
  if (consulta.isPending) return <Skeleton className="h-72" />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  const r = consulta.data

  return (
    <div className="grid gap-4">
      <LineaPeriodo periodo={r.periodo} cargando={consulta.isFetching} />
      {r.doctores.length === 0 ? (
        <p className="rounded-[10px] border border-dashed border-linea px-6 py-10 text-pizarra">Sin datos en el periodo.</p>
      ) : (
        <div className={CONTENEDOR_TABLA}>
          <Table>
            <TableHeader>
              <TableRow className={ENCABEZADO_TABLA}>
                <TableHead className="pl-4">Doctor</TableHead>
                <TableHead className="text-right">Citas</TableHead>
                <TableHead className="hidden text-right sm:table-cell">Completadas</TableHead>
                <TableHead className="hidden text-right sm:table-cell">No asistió</TableHead>
                <TableHead className="hidden text-right md:table-cell">Canceladas</TableHead>
                <TableHead className="hidden text-right md:table-cell">Por atender</TableHead>
                <TableHead className="hidden text-right lg:table-cell">Evoluciones</TableHead>
                <TableHead className="pr-4">Asistencia</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {r.doctores.map((d) => (
                <TableRow key={d.idDoctor}>
                  <TableCell className="py-3 pl-4">
                    <p className="font-medium">{d.nombreDoctor}</p>
                    <p className="text-xs text-pizarra">{d.especialidad}</p>
                  </TableCell>
                  <TableCell className="cifras text-right font-medium">{d.citasTotales}</TableCell>
                  <TableCell className="cifras hidden text-right sm:table-cell">{d.completadas}</TableCell>
                  <TableCell className="cifras hidden text-right sm:table-cell">{d.noAsistio}</TableCell>
                  <TableCell className="cifras hidden text-right md:table-cell">{d.canceladas}</TableCell>
                  <TableCell className="cifras hidden text-right md:table-cell">{d.porAtender}</TableCell>
                  <TableCell className="cifras hidden text-right lg:table-cell">{d.evoluciones}</TableCell>
                  <TableCell className="pr-4">
                    {d.tasaAsistencia === null ? (
                      <span className="text-pizarra">—</span>
                    ) : (
                      <div className="flex min-w-28 items-center gap-2">
                        <div className="h-1.5 flex-1 overflow-hidden rounded-full bg-papel" aria-hidden="true">
                          <div
                            className="h-full rounded-full bg-estado-completada"
                            style={{ width: `${d.tasaAsistencia}%` }}
                          />
                        </div>
                        <span className="cifras w-12 text-right text-tinta">
                          {formatearCifra(d.tasaAsistencia, 'porcentaje')}
                        </span>
                      </div>
                    )}
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </div>
      )}
      <p className="text-xs text-pizarra">
        Asistencia: citas completadas entre completadas más inasistencias. Las canceladas y las que están por atender no cuentan.
      </p>
    </div>
  )
}

// ============================================
// 4. Pacientes
// ============================================

function ReporteDePacientes({ filtro }: { filtro: FiltroReporte }) {
  const consulta = useReporte<ReportePacientes>('pacientes', filtro, true)
  if (consulta.isPending) return <Esqueleto />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  const r = consulta.data

  return (
    <div className="grid gap-4">
      <LineaPeriodo periodo={r.periodo} cargando={consulta.isFetching} />
      <dl className="grid grid-cols-2 gap-px overflow-hidden rounded-[10px] border border-linea bg-linea">
        <Cifra titulo="Pacientes activos" valor={formatearCifra(r.pacientesActivos)} detalle="A la fecha" />
        <Cifra titulo="Nuevos en el periodo" valor={formatearCifra(r.nuevosEnPeriodo)} detalle="Registrados en estas fechas" />
      </dl>
      <div className="grid gap-4 md:grid-cols-3">
        <Tarjeta titulo="Por género" descripcion="Pacientes activos">
          <BarrasConteo conteos={r.porGenero} />
        </Tarjeta>
        <Tarjeta titulo="Por rango de edad" descripcion="Años cumplidos">
          <BarrasConteo conteos={r.porRangoEdad} />
        </Tarjeta>
        <Tarjeta titulo="Por grupo sanguíneo">
          <BarrasConteo conteos={r.porGrupoSanguineo} />
        </Tarjeta>
      </div>
    </div>
  )
}

// ============================================
// 5. Diagnósticos
// ============================================

function ReporteDeDiagnosticos({ filtro }: { filtro: FiltroReporte }) {
  const consulta = useReporte<ReporteDiagnosticos>('diagnosticos', filtro, true)
  if (consulta.isPending) return <Skeleton className="h-80" />
  if (consulta.isError) return <ErrorVista error={consulta.error} alReintentar={() => consulta.refetch()} />
  const r = consulta.data

  return (
    <div className="grid gap-4">
      <LineaPeriodo periodo={r.periodo} cargando={consulta.isFetching} />
      <Tarjeta
        titulo="Diagnósticos más frecuentes"
        descripcion={`${formatearCifra(r.totalEvoluciones)} evoluciones registradas en el periodo`}
      >
        <BarrasConteo conteos={r.diagnosticos} vacio="No se registraron evoluciones en el periodo." />
      </Tarjeta>
    </div>
  )
}
