# SampleBlog Frontend

A Vue 3 + TypeScript frontend application for the SampleBlog project.

## Features

- ⚡ **Vue 3** - Progressive JavaScript framework with Composition API
- 🏷️ **TypeScript** - Typed superset of JavaScript
- 🚀 **Vite** - Fast build tool and development server
- 🛣️ **Vue Router** - Official router for Vue.js
- 📱 **Responsive Design** - Mobile-first design approach
- 🎨 **Modern CSS** - Clean and modern styling

## Getting Started

### Prerequisites

- Node.js (version 18 or higher)
- npm or yarn package manager

### Installation

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

### Development

Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:3000`

### Build for Production

Build the application for production:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

### Linting

Run ESLint to check for code quality issues:
```bash
npm run lint
```

## Project Structure

```
frontend/
├── public/                 # Static assets
├── src/
│   ├── components/        # Reusable Vue components
│   │   └── HelloWorld.vue # Example component
│   ├── views/            # Page components
│   │   ├── Home.vue      # Home page
│   │   └── About.vue     # About page
│   ├── router/           # Vue Router configuration
│   │   └── index.ts      # Router setup
│   ├── App.vue           # Root component
│   ├── main.ts           # Application entry point
│   ├── style.css         # Global styles
│   └── shims-vue.d.ts    # TypeScript declarations
├── index.html            # HTML template
├── package.json          # Dependencies and scripts
├── tsconfig.json         # TypeScript configuration
├── vite.config.ts        # Vite configuration
└── README.md            # This file
```

## API Integration

The Vite configuration includes a proxy setup that forwards API requests to the backend:

- Frontend: `http://localhost:3000`
- Backend API: `http://localhost:5000` (proxied as `/api/*`)

## Technology Stack

- **Vue 3** - Reactive frontend framework
- **TypeScript** - Static type checking
- **Vite** - Build tool and dev server
- **Vue Router** - Client-side routing
- **ESLint** - Code linting and formatting

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Next Steps

1. Install dependencies: `npm install`
2. Start development server: `npm run dev`
3. Begin building your blog frontend!

The current setup provides a solid foundation with:
- A responsive layout with header, main content, and footer
- Vue Router for navigation
- TypeScript for type safety
- A "Hello World" example with interactive features
- Modern CSS styling with utility classes