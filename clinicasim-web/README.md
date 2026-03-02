# ClinicaSim Web (Tarea 5)

Frontend Angular base para ClinicaSim.

## Requisitos

- Node.js 20+
- npm 10+

## Instalar dependencias

```bash
cd clinicasim-web
npm install
```

## Ejecutar en desarrollo

```bash
npm run start
```

Aplicación disponible en `http://localhost:4200`.

## Configurar API

Editar:
- `src/environments/environment.ts`
- `src/environments/environment.development.ts`

Propiedad:

```ts
apiBaseUrl: 'http://localhost:5000'
```

Ajustar al puerto real del backend .NET si difiere.

## Rutas base

- `/` Home
- `/cases` Lista de casos
- `/session/:sessionCode` Sesión
