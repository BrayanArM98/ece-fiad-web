import { isToday } from 'date-fns'
import { useEffect, useState } from 'react'
import { ESTADOS_CITA } from '@/lib/estados'
import { formatearHora } from '@/lib/formato'
import type { Cita } from '@/lib/tipos'
import { cn } from '@/lib/utils'
import { DURACION_CITA_MIN, HORA_FIN, HORA_INICIO, minutosDelDia, textoHora } from './fechas'

/** Alto en píxeles de una hora: media hora alcanza para dos líneas de texto. */
const PX_POR_HORA = 96
const PX_POR_MINUTO = PX_POR_HORA / 60

interface CitaUbicada {
  cita: Cita
  inicio: number
  columna: number
  columnas: number
}

/**
 * Acomoda las citas que se enciman (mismo horario con distintos doctores) en columnas,
 * como en cualquier calendario: cada grupo de citas encimadas se reparte el ancho.
 */
function ubicarCitas(citas: Cita[]): CitaUbicada[] {
  const ordenadas = citas
    .map((cita) => ({ cita, inicio: minutosDelDia(cita.fechaHora) }))
    .sort((a, b) => a.inicio - b.inicio)

  const resultado: CitaUbicada[] = []
  let grupo: CitaUbicada[] = []
  let finesPorColumna: number[] = []
  let finGrupo = -1

  const cerrarGrupo = () => {
    for (const ubicada of grupo) ubicada.columnas = finesPorColumna.length
    resultado.push(...grupo)
    grupo = []
    finesPorColumna = []
  }

  for (const { cita, inicio } of ordenadas) {
    if (inicio >= finGrupo) cerrarGrupo()
    const fin = inicio + DURACION_CITA_MIN
    let columna = finesPorColumna.findIndex((finColumna) => finColumna <= inicio)
    if (columna === -1) {
      columna = finesPorColumna.length
      finesPorColumna.push(fin)
    } else {
      finesPorColumna[columna] = fin
    }
    grupo.push({ cita, inicio, columna, columnas: 1 })
    finGrupo = Math.max(finGrupo, fin)
  }
  cerrarGrupo()
  return resultado
}

/** Minutos desde la medianoche, actualizados cada minuto, para la línea de "ahora". */
function useMinutoActual() {
  const [minuto, setMinuto] = useState(() => minutosDelDia(new Date()))
  useEffect(() => {
    const intervalo = window.setInterval(() => setMinuto(minutosDelDia(new Date())), 60_000)
    return () => window.clearInterval(intervalo)
  }, [])
  return minuto
}

export function VistaDia({
  fecha,
  citas,
  mostrarDoctor,
  alAbrir,
  alAgendarEn,
}: {
  fecha: Date
  citas: Cita[]
  mostrarDoctor: boolean
  alAbrir: (cita: Cita) => void
  /** Si se recibe, un clic en un espacio libre futuro abre "Agendar cita" con ese horario. */
  alAgendarEn?: (horario: string) => void
}) {
  const minutoActual = useMinutoActual()
  const ubicadas = ubicarCitas(citas)

  // El horario visible se amplía si alguna cita queda fuera de 08:00 a 20:00
  const primera = Math.min(HORA_INICIO * 60, ...ubicadas.map((u) => u.inicio))
  const ultima = Math.max(HORA_FIN * 60, ...ubicadas.map((u) => u.inicio + DURACION_CITA_MIN))
  const horaInicio = Math.floor(primera / 60)
  const horaFin = Math.ceil(ultima / 60)
  const horas = Array.from({ length: horaFin - horaInicio }, (_, i) => horaInicio + i)
  const alto = (horaFin - horaInicio) * PX_POR_HORA
  const aPixeles = (minutos: number) => (minutos - horaInicio * 60) * PX_POR_MINUTO

  const esHoy = isToday(fecha)
  const mostrarAhora = esHoy && minutoActual >= horaInicio * 60 && minutoActual <= horaFin * 60
  const espacios = horas.flatMap((h) => [h * 60, h * 60 + 30])
  const ahora = new Date()
  const esFuturo = (minutos: number) =>
    new Date(fecha.getFullYear(), fecha.getMonth(), fecha.getDate(), 0, minutos) > ahora

  return (
    <div className="rounded-[10px] border border-linea bg-card py-4 pr-3 sm:pr-4">
      <div className="relative grid grid-cols-[3.75rem_1fr]" style={{ height: alto }}>
        {/* Regla de horas */}
        <div className="relative" aria-hidden="true">
          {horas.map((h) => (
            <span
              key={h}
              className="cifras absolute right-3 -translate-y-1/2 text-xs text-pizarra"
              style={{ top: aPixeles(h * 60) }}
            >
              {textoHora(h * 60)}
            </span>
          ))}
        </div>

        <div className="relative">
          {/* Líneas de cada hora y media hora */}
          {horas.map((h) => (
            <div key={h} aria-hidden="true">
              <div className="absolute inset-x-0 border-t border-linea" style={{ top: aPixeles(h * 60) }} />
              <div
                className="absolute inset-x-0 border-t border-dashed border-linea/70"
                style={{ top: aPixeles(h * 60 + 30) }}
              />
            </div>
          ))}
          <div className="absolute inset-x-0 border-t border-linea" style={{ top: alto }} aria-hidden="true" />

          {/* Espacios libres: atajo con el mouse; con el teclado se usa el botón "Agendar cita" */}
          {alAgendarEn
            ? espacios.filter(esFuturo).map((minutos) => (
                <button
                  key={minutos}
                  type="button"
                  tabIndex={-1}
                  aria-hidden="true"
                  title={`Agendar a las ${textoHora(minutos)}`}
                  onClick={() => alAgendarEn(textoHora(minutos))}
                  className="group/espacio absolute inset-x-0 flex items-center px-3 text-xs text-transparent hover:bg-marea-suave/60 hover:text-marea"
                  style={{ top: aPixeles(minutos), height: 30 * PX_POR_MINUTO }}
                >
                  + Agendar a las {textoHora(minutos)}
                </button>
              ))
            : null}

          {/* Citas */}
          <ol aria-label="Citas del día">
            {ubicadas.map((u) => (
              <li
                key={u.cita.id}
                className="absolute px-0.5"
                style={{
                  top: aPixeles(u.inicio) + 1,
                  height: DURACION_CITA_MIN * PX_POR_MINUTO - 2,
                  left: `${(u.columna / u.columnas) * 100}%`,
                  width: `${100 / u.columnas}%`,
                }}
              >
                <BloqueCita cita={u.cita} mostrarDoctor={mostrarDoctor} alAbrir={alAbrir} />
              </li>
            ))}
          </ol>

          {mostrarAhora ? (
            <div
              className="pointer-events-none absolute inset-x-0 z-10 flex items-center"
              style={{ top: aPixeles(minutoActual) }}
              aria-hidden="true"
            >
              <span className="-ml-1 size-2 rounded-full bg-marea" />
              <span className="h-px flex-1 bg-marea" />
            </div>
          ) : null}
        </div>
      </div>
    </div>
  )
}

function BloqueCita({
  cita,
  mostrarDoctor,
  alAbrir,
}: {
  cita: Cita
  mostrarDoctor: boolean
  alAbrir: (cita: Cita) => void
}) {
  const estado = ESTADOS_CITA[cita.estado]
  const cancelada = cita.estado === 'Cancelada'
  const detalle = mostrarDoctor ? cita.nombreDoctor : cita.motivo || 'Sin motivo registrado'

  return (
    <button
      type="button"
      onClick={() => alAbrir(cita)}
      className={cn(
        '@container relative flex size-full flex-col justify-center overflow-hidden rounded-md border border-linea py-1 pr-2 pl-3 text-left transition-colors hover:border-marea focus-visible:ring-3 focus-visible:ring-ring/50 focus-visible:outline-none',
        cancelada ? 'bg-papel' : 'bg-card shadow-[0_1px_0_var(--color-linea)]',
      )}
    >
      <span className={cn('absolute inset-y-0 left-0 w-1', estado.barra)} aria-hidden="true" />
      <span className="flex min-w-0 items-baseline gap-2">
        <span className="cifras shrink-0 text-xs font-medium text-tinta">{formatearHora(cita.fechaHora)}</span>
        <span className={cn('truncate text-[0.8125rem] font-medium', cancelada && 'text-pizarra line-through')}>
          {cita.nombrePaciente}
        </span>
        <span className="ml-auto hidden shrink-0 text-xs text-pizarra @[18rem]:inline">{estado.texto}</span>
      </span>
      <span className="truncate text-xs text-pizarra">
        <span className="sr-only">{estado.texto}. </span>
        {detalle}
      </span>
    </button>
  )
}
