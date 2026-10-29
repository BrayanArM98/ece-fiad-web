using Dominio.Entidades.Citas;
using Dominio.Entidades.Doctores;
using Dominio.Entidades.Especialidades;
using Dominio.Entidades.Evoluciones;
using Dominio.Entidades.HistoriasClinicas;
using Dominio.Entidades.Pacientes;
using Dominio.Entidades.Usuarios;
using Dominio.Enumeraciones;
using Microsoft.EntityFrameworkCore;

namespace Infraestructura.Data
{
    /// <summary>
    /// Datos de demostración para una base nueva: especialidades, doctores, pacientes,
    /// historias clínicas, evoluciones y citas de los últimos 60 días y las próximas 2 semanas.
    /// Sirven para presentar el sistema, probar la app móvil y tener información para los reportes.
    /// Nunca modifican datos existentes: el catálogo solo se crea si no hay doctores ni pacientes.
    /// </summary>
    public static class SembradorDatosDemostracion
    {
        // Semilla fija: cada base nueva recibe exactamente los mismos datos
        private const int Semilla = 2026;

        private static readonly (string Nombre, string Descripcion)[] Especialidades =
        {
            ("Medicina General", "Atención primaria y control de enfermedades comunes."),
            ("Pediatría", "Atención médica de niños y adolescentes."),
            ("Cardiología", "Diagnóstico y tratamiento de enfermedades del corazón."),
            ("Dermatología", "Enfermedades de la piel, el cabello y las uñas."),
            ("Ginecología", "Salud reproductiva y atención de la mujer.")
        };

        private static readonly (string Nombres, string Apellidos, string Telefono, string Email, string Horario, int Especialidad)[] Doctores =
        {
            ("Ana Lucía", "López Herrera", "6461001001", "ana.lopez@ecefiad.mx", "Lunes a viernes, 8:00 a 14:00", 0),
            ("Carlos", "Méndez Ruiz", "6461001002", "carlos.mendez@ecefiad.mx", "Lunes a viernes, 14:00 a 20:00", 2),
            ("María Fernanda", "Torres Vega", "6461001003", "maria.torres@ecefiad.mx", "Lunes a viernes, 9:00 a 15:00", 1),
            ("Jorge", "Ramírez Soto", "6461001004", "jorge.ramirez@ecefiad.mx", "Martes a sábado, 10:00 a 16:00", 3),
            ("Lucía", "Castillo Navarro", "6461001005", "lucia.castillo@ecefiad.mx", "Lunes a viernes, 8:00 a 13:00", 4)
        };

        private static readonly (string Nombres, string Apellidos, string Nacimiento, Genero Genero, GrupoSanguineo Sangre)[] Pacientes =
        {
            ("Juan", "Pérez Gómez", "1985-04-12", Genero.Masculino, GrupoSanguineo.OPositivo),
            ("María José", "Hernández Ruiz", "1992-09-03", Genero.Femenino, GrupoSanguineo.APositivo),
            ("Luis Alberto", "Martínez Díaz", "1978-01-25", Genero.Masculino, GrupoSanguineo.BPositivo),
            ("Sofía", "García Morales", "2015-06-18", Genero.Femenino, GrupoSanguineo.OPositivo),
            ("Pedro", "Sánchez Cruz", "1960-11-30", Genero.Masculino, GrupoSanguineo.ANegativo),
            ("Valeria", "Ramírez Flores", "2001-03-07", Genero.Femenino, GrupoSanguineo.ABPositivo),
            ("Diego", "Jiménez Ortiz", "1995-08-22", Genero.Masculino, GrupoSanguineo.OPositivo),
            ("Camila", "Reyes Mendoza", "1988-12-14", Genero.Femenino, GrupoSanguineo.BNegativo),
            ("Roberto", "Díaz Aguilar", "1970-05-09", Genero.Masculino, GrupoSanguineo.ONegativo),
            ("Fernanda", "Vargas Silva", "2010-02-28", Genero.Femenino, GrupoSanguineo.APositivo)
        };

        private static readonly (string Alergias, string Familiares, string Personales)[] Historias =
        {
            ("Penicilina", "Padre con diabetes tipo 2", "Hipertensión controlada desde 2020"),
            ("Ninguna conocida", "Madre con hipotiroidismo", "Sin antecedentes relevantes"),
            ("Ácido acetilsalicílico", "Abuelo con infarto al miocardio", "Colesterol elevado"),
            ("Polen y ácaros", "Sin antecedentes relevantes", "Asma leve intermitente"),
            ("Ninguna conocida", "Hermano con hipertensión", "Diabetes tipo 2 en tratamiento"),
            ("Látex", "Madre con migraña", "Dermatitis atópica"),
            ("Ninguna conocida", "Sin antecedentes relevantes", "Fractura de radio en 2018"),
            ("Sulfas", "Madre con cáncer de mama", "Ciclos menstruales irregulares")
        };

        private static readonly string[] Motivos =
        {
            "Consulta de control", "Dolor de cabeza frecuente", "Revisión de resultados de laboratorio",
            "Seguimiento de tratamiento", "Dolor abdominal", "Revisión de presión arterial",
            "Valoración general", "Erupción en la piel", "Control de peso", "Tos persistente"
        };

        // Diagnóstico y tratamiento de ejemplo por especialidad (índice del arreglo Especialidades)
        private static readonly (string Diagnostico, string Tratamiento)[][] Notas =
        {
            new[] { ("Infección respiratoria alta", "Paracetamol 500 mg cada 8 horas por 5 días, reposo e hidratación"),
                    ("Gastritis aguda", "Omeprazol 20 mg en ayunas por 14 días y dieta blanda"),
                    ("Hipertensión arterial controlada", "Continuar losartán 50 mg diario y reducir el consumo de sal") },
            new[] { ("Faringitis viral", "Ibuprofeno pediátrico según peso y líquidos abundantes"),
                    ("Control de niño sano", "Esquema de vacunación al día; próxima revisión en 6 meses") },
            new[] { ("Arritmia sinusal benigna", "Sin tratamiento farmacológico; electrocardiograma de control en 3 meses"),
                    ("Hipercolesterolemia", "Atorvastatina 20 mg diaria y plan de alimentación") },
            new[] { ("Dermatitis atópica", "Hidratación diaria e hidrocortisona al 1% por 7 días"),
                    ("Acné moderado", "Peróxido de benzoilo al 5% por las noches") },
            new[] { ("Revisión ginecológica anual normal", "Papanicolaou de rutina; control en 12 meses"),
                    ("Síndrome premenstrual", "Ejercicio regular y suplemento de magnesio") }
        };

        /// <summary>
        /// Crea el catálogo completo solo si la base no tiene doctores ni pacientes.
        /// </summary>
        /// <returns>true si se crearon datos.</returns>
        public static async Task<bool> SembrarCatalogoAsync(ContextoECE contexto, DateTime ahora)
        {
            if (await contexto.Doctores.AnyAsync() || await contexto.Pacientes.AnyAsync())
                return false;

            var aleatorio = new Random(Semilla);

            // 1. Especialidades (se reutilizan las que ya existan con el mismo nombre)
            var especialidades = new List<Especialidad>();
            foreach (var (nombre, descripcion) in Especialidades)
            {
                var existente = await contexto.Especialidades.FirstOrDefaultAsync(e => e.Nombre == nombre);
                if (existente == null)
                {
                    existente = new Especialidad { Nombre = nombre, Descripcion = descripcion };
                    contexto.Especialidades.Add(existente);
                }
                especialidades.Add(existente);
            }

            // 2. Doctores
            var doctores = Doctores.Select(d => new Doctor
            {
                Nombres = d.Nombres,
                Apellidos = d.Apellidos,
                Telefono = d.Telefono,
                Email = d.Email,
                HorarioAtencion = d.Horario,
                Especialidad = especialidades[d.Especialidad]
            }).ToList();
            contexto.Doctores.AddRange(doctores);

            // 3. Pacientes
            var pacientes = Pacientes.Select((p, i) => new Paciente
            {
                Nombres = p.Nombres,
                Apellidos = p.Apellidos,
                NumeroDocumento = $"DEMO{i + 1:0000}",
                TipoDocumento = TipoDeDocumento.Cedula,
                Telefono = $"64620020{i + 1:00}",
                Email = $"paciente{i + 1}@correo-demo.mx",
                FechaNacimiento = DateTime.Parse(p.Nacimiento),
                Genero = p.Genero,
                GrupoSanguineo = p.Sangre,
                Direccion = $"Calle {i + 1} #{100 + i * 7}, Ensenada, B.C."
            }).ToList();
            contexto.Pacientes.AddRange(pacientes);

            // 4. Historias clínicas para los primeros 8 pacientes
            var historias = Historias.Select((h, i) => new HistoriaClinica
            {
                Paciente = pacientes[i],
                FechaApertura = ahora.Date.AddDays(-120 + i * 3),
                Alergias = h.Alergias,
                AntecedentesFamiliares = h.Familiares,
                AntecedentesPersonales = h.Personales
            }).ToList();
            contexto.HistoriasClinicas.AddRange(historias);

            // 5. Citas: días hábiles de los últimos 60 días y las próximas 2 semanas
            var ocupados = new HashSet<(Doctor, DateTime)>();
            var citas = new List<Cita>();

            Cita? Agendar(Paciente paciente, Doctor doctor, DateTime fechaHora, EstadoCita estado)
            {
                if (!ocupados.Add((doctor, fechaHora)))
                    return null;

                var cita = new Cita
                {
                    Paciente = paciente,
                    Doctor = doctor,
                    FechaHora = fechaHora,
                    Motivo = Motivos[aleatorio.Next(Motivos.Length)],
                    Estado = estado,
                    FechaDeCreacion = fechaHora.AddDays(-aleatorio.Next(1, 10))
                };
                citas.Add(cita);
                return cita;
            }

            for (var dia = -60; dia <= 14; dia++)
            {
                var fecha = ahora.Date.AddDays(dia);
                if (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
                    continue;

                var cantidad = aleatorio.Next(3, 7);
                for (var n = 0; n < cantidad; n++)
                {
                    var fechaHora = fecha.AddHours(aleatorio.Next(9, 18));
                    Agendar(
                        pacientes[aleatorio.Next(pacientes.Count)],
                        doctores[aleatorio.Next(doctores.Count)],
                        fechaHora,
                        EstadoSegunFecha(fechaHora, ahora, aleatorio));
                }
            }

            // Agenda de hoy para la primera doctora y próximas citas del primer paciente
            // (las cuentas de demostración de médico y paciente se vinculan con ellos)
            foreach (var hora in new[] { 9, 11, 13 })
                Agendar(pacientes[hora % pacientes.Count], doctores[0], ahora.Date.AddHours(hora),
                    EstadoSegunFecha(ahora.Date.AddHours(hora), ahora, aleatorio));

            Agendar(pacientes[0], doctores[0], ahora.Date.AddDays(1).AddHours(10), EstadoCita.Confirmada);
            Agendar(pacientes[0], doctores[1], ahora.Date.AddDays(5).AddHours(16), EstadoCita.Pendiente);

            contexto.Citas.AddRange(citas);

            // 6. Evoluciones: la mitad de las citas completadas de pacientes con historia
            foreach (var cita in citas.Where(c => c.Estado == EstadoCita.Completada))
            {
                var historia = historias.FirstOrDefault(h => h.Paciente == cita.Paciente);
                if (historia == null || aleatorio.Next(2) == 0)
                    continue;

                var opciones = Notas[especialidades.IndexOf(cita.Doctor.Especialidad)];
                var (diagnostico, tratamiento) = opciones[aleatorio.Next(opciones.Length)];

                contexto.Evoluciones.Add(new Evolucion
                {
                    HistoriaClinica = historia,
                    Doctor = cita.Doctor,
                    Fecha = cita.FechaHora.AddMinutes(30),
                    Diagnostico = diagnostico,
                    Tratamiento = tratamiento,
                    Notas = $"Motivo de consulta: {cita.Motivo.ToLowerInvariant()}."
                });
            }

            await contexto.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Crea las cuentas de demostración (recepción, médico y paciente) si no existen.
        /// El médico se vincula con el primer doctor sin cuenta y el paciente con el primer paciente sin cuenta.
        /// </summary>
        /// <returns>Los correos de las cuentas creadas.</returns>
        public static async Task<List<string>> SembrarUsuariosAsync(
            ContextoECE contexto,
            Func<string, string> cifrarContrasena,
            string contrasena)
        {
            var creados = new List<string>();
            var hash = cifrarContrasena(contrasena);

            async Task Crear(string email, string nombre, RolUsuario rol, int? idDoctor = null, int? idPaciente = null)
            {
                if (await contexto.Usuarios.AnyAsync(u => u.Email == email))
                    return;

                contexto.Usuarios.Add(new Usuario
                {
                    Email = email,
                    NombreCompleto = nombre,
                    Rol = rol,
                    IdDoctor = idDoctor,
                    IdPaciente = idPaciente,
                    ContrasenaHash = hash
                });
                creados.Add(email);
            }

            await Crear("recepcion@ecefiad.mx", "Recepción ECE-FIAD", RolUsuario.Recepcionista);

            var doctor = await contexto.Doctores
                .Where(d => d.Activo && !d.Eliminado && !contexto.Usuarios.Any(u => u.IdDoctor == d.Id))
                .OrderBy(d => d.Id)
                .FirstOrDefaultAsync();
            if (doctor != null)
                await Crear("medico@ecefiad.mx", $"{doctor.Nombres} {doctor.Apellidos}", RolUsuario.Medico, idDoctor: doctor.Id);

            var paciente = await contexto.Pacientes
                .Where(p => p.Activo && !p.Eliminado && !contexto.Usuarios.Any(u => u.IdPaciente == p.Id))
                .OrderBy(p => p.Id)
                .FirstOrDefaultAsync();
            if (paciente != null)
                await Crear("paciente@ecefiad.mx", $"{paciente.Nombres} {paciente.Apellidos}", RolUsuario.Paciente, idPaciente: paciente.Id);

            await contexto.SaveChangesAsync();
            return creados;
        }

        private static EstadoCita EstadoSegunFecha(DateTime fechaHora, DateTime ahora, Random aleatorio)
        {
            if (fechaHora > ahora)
                return aleatorio.Next(10) < 6 ? EstadoCita.Pendiente : EstadoCita.Confirmada;

            var valor = aleatorio.Next(100);
            return valor switch
            {
                < 75 => EstadoCita.Completada,
                < 87 => EstadoCita.NoAsistio,
                _ => EstadoCita.Cancelada
            };
        }
    }
}
