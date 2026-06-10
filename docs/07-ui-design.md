# 07 — UI/UX Design System

## 1. Design Reference

The frontend prototype at `student-groups-hub-fronted/` serves as the **authoritative visual reference** for this project. All new UI must follow the patterns and tokens established there.

Prototype documentation: `student-groups-hub-fronted/docs/branding/style-guide.md`

---

## 2. Brand Identity

- **University:** Universidad Regiomontana (U-ERRE)
- **Tone:** Professional but approachable. Casual compared to the institutional U-ERRE site. Emphasis on community and student social life.
- **Language:** Spanish (all UI text, labels, error messages, notifications)
- **Font strategy:** Montserrat for headings, Open Sans for body and UI labels

---

## 3. Color Palette

### Primary (U-ERRE Purple)

| Token | Hex | Usage |
|---|---|---|
| `--color-uerre-purple` | `#5B21B6` | Primary brand, buttons, links |
| `--color-uerre-purple-dark` | `#4C1D95` | Hover states, hero gradient start |
| `--color-uerre-purple-light` | `#7C3AED` | Accents, gradients |
| `--color-uerre-purple-vivid` | `#8B5CF6` | Gradient endpoints |

### Secondary

| Token | Hex | Usage |
|---|---|---|
| `--color-uerre-navy` | `#1E1B4B` | Dark backgrounds |
| `--color-uerre-navy-deep` | `#0F0A2A` | Footer background |
| `--color-uerre-gold` | `#F59E0B` | Accent highlights, date badges, RSVP |
| `--color-uerre-gold-light` | `#FBBF24` | Gold accents |

### Neutral

| Token | Hex | Usage |
|---|---|---|
| `--color-uerre-surface` | `#F3F4F6` | Section backgrounds |
| `--color-uerre-text` | `#374151` | Body text |
| `--color-uerre-heading` | `#111827` | Headings |
| `--color-uerre-muted` | `#9CA3AF` | Secondary/placeholder text |
| `--color-uerre-tag-bg` | `#EDE9FE` | Category tag backgrounds |
| `--color-uerre-tag-text` | `#6D28D9` | Category tag text |

### Gradients

```css
/* Hero / CTA sections */
background: linear-gradient(135deg, #4C1D95, #7C3AED, #8B5CF6);

/* Primary CTA button */
background: linear-gradient(90deg, #5B21B6, #7C3AED);

/* Footer / dark sections */
background: linear-gradient(180deg, #1E1B4B, #0F0A2A);
```

---

## 4. Typography

| Role | Font | Weight | Size | Line Height |
|---|---|---|---|---|
| H1 | Montserrat | 700 | 2.5rem | 1.2 |
| H2 | Montserrat | 700 | 2rem | 1.25 |
| H3 | Montserrat | 600 | 1.5rem | 1.3 |
| H4 | Montserrat | 600 | 1.25rem | 1.35 |
| Body | Open Sans | 400 | 1rem | 1.6 |
| Small | Open Sans | 400 | 0.875rem | 1.5 |
| XS / Badge | Open Sans | 500 | 0.75rem | 1.4 |

CSS variables: `--font-montserrat`, `--font-open-sans`
Tailwind classes: `font-heading` (Montserrat), `font-sans` (Open Sans)

---

## 5. Spacing System (4px base unit)

| Token | Value | Usage |
|---|---|---|
| `xs` | 4px | Tight inner spacing |
| `sm` | 8px | Icon-to-text gaps |
| `md` | 16px | Component padding |
| `lg` | 24px | Card padding, section gap |
| `xl` | 32px | Between sections |
| `2xl` | 48px | Major section spacing |
| `3xl` | 64px | Hero/CTA vertical padding |
| `4xl` | 96px | Page-level section separation |

---

## 6. Border Radius

| Token | Value | Usage |
|---|---|---|
| `sm` | 4px (6px via `--radius-sm`) | Tags, badges |
| `md` | 8px | Buttons, inputs |
| `lg` | 12px | Cards, modals |
| `xl` | 16px | Feature cards, hero sections |
| `full` | 9999px | Avatars, round badges |

---

## 7. Shadows

| Token | Definition | Usage |
|---|---|---|
| `sm` | `0 1px 2px rgba(0,0,0,0.05)` | Subtle elevation |
| `md` | `0 4px 6px rgba(0,0,0,0.07)` | Cards, dropdowns |
| `lg` | `0 10px 15px rgba(0,0,0,0.1)` | Modals, popovers |
| `purple` | `0 4px 14px rgba(91,33,182,0.3)` | Purple CTA buttons |

---

## 8. Component Patterns

### Buttons

| Variant | Background | Text | Usage |
|---|---|---|---|
| Primary | `#5B21B6` | White | Main actions (Unirse, Crear, Enviar) |
| Secondary | Transparent | `#5B21B6` border | Secondary actions (Cancelar, Volver) |
| Ghost | Transparent | `#5B21B6` | Nav links, tertiary |
| Accent | `#F59E0B` | `#111827` | Urgent CTAs, highlights |
| Destructive | `#EF4444` | White | Delete, remove, reject |

States: hover (darken 10% + shadow-purple), disabled (50% opacity), loading (spinner).

### Cards

- Background: white
- Border: `1px solid #E5E7EB`
- Border radius: `lg` (12px)
- Shadow: `md`
- Padding: `lg` (24px)
- Hover: shadow-lg + `scale(1.02)`

### Group Cards

```
┌─────────────────────────────────┐
│  [banner/logo image area 4:3]   │
├─────────────────────────────────┤
│  [category tag pill]            │
│  Group Name (H3)                │
│  Short description (2 lines)    │
│  ─────────────────────────────  │
│  👥 48 miembros    [Unirse →]   │
└─────────────────────────────────┘
```

### Event Cards

```
┌──────────────────────────────────┐
│  [gold date badge]  [status tag] │
│  Event Title (H3)                │
│  Group name • Location           │
│  📅 15 Sep, 09:00 — 17:00       │
│  ─────────────────────────────── │
│  👥 32/60 spots  [RSVP button]  │
└──────────────────────────────────┘
```

### Category Tags

```
[  Tecnología e Innovación  ]
```
- Background: `#EDE9FE`
- Text: `#6D28D9`
- Border radius: full (pill)
- Font: XS, 500

### Navigation Bar

- Height: 64px
- Background: white (solid) / sticky top
- Left: Logo (purple "U" box + "Student Groups" / "U-ERRE")
- Center: Nav links (Inicio, Grupos, Eventos, Comunidad)
- Right: notification bell + user avatar (when authenticated) OR "Iniciar Sesión" button

### Notification Badge

- Gold circle (`#F59E0B`) with white number on top of bell icon
- Hidden when count is 0

---

## 9. Page Layouts

### Landing Page (`/`)

1. **Hero Section** — full-width purple gradient, centered heading + CTA buttons + floating group preview cards
2. **Stats Band** — dark navy band with counters (120+ grupos, 3,400 estudiantes, 280 eventos, 95% satisfacción)
3. **Featured Groups Grid** — `#grupos` anchor, 3-column card grid
4. **Upcoming Events** — `#eventos` anchor, 2×2 event card grid
5. **CTA Banner** — purple gradient, "Únete a la comunidad" + signup button
6. **Footer**

### Groups Listing (`/groups`)

- Search bar + category filter chips
- Responsive grid of `GroupCard` components
- Pagination or infinite scroll
- Empty state illustration with CTA

### Group Detail (`/groups/[slug]`)

- Banner image + group header (logo, name, category, member count, join button)
- Tabs: Información | Miembros | Eventos | Posts (future)
- Sidebar (on desktop): contact info, leader info

### Dashboard (`/dashboard`)

- Greeting card with user name
- Role-specific widgets (see 06-modules.md §9.7)
- Quick action cards

### Admin Panel (`/admin/*`)

- Sidebar navigation
- Data tables with search, filter, pagination
- Status badges with color coding
- Approve/reject inline actions

---

## 10. Animations

| Class | Effect | Duration | Usage |
|---|---|---|---|
| `animate-fade-up` | Slide up + fade in | 0.7s | Section entries |
| `animate-fade-in` | Fade in | 0.6s | Cards, modals |
| `animate-float` | Gentle bob | 4s infinite | Hero decorative cards |
| `animate-count-up` | Slide up + fade in | 0.6s | Stats counters |
| `delay-100` to `delay-500` | Staggered delays | — | Grid children |

---

## 11. Responsive Breakpoints

| Breakpoint | Min width | Columns | Notes |
|---|---|---|---|
| Mobile | `< 640px` | 1 | Stack all cards, hamburger menu |
| Tablet (`md`) | `768px` | 2 | 2-column grids, expanded nav |
| Desktop (`lg`) | `1024px` | 3 | Standard layout |
| Wide (`xl`) | `1280px` | 4 | Wide group/event grids |

Max content width: **1200px**, centered.

---

## 12. Accessibility Guidelines

- All interactive elements must be keyboard-accessible (Tab + Enter/Space)
- All images must have descriptive `alt` text (or `alt=""` for decorative)
- Color contrast: minimum WCAG AA (4.5:1 for normal text, 3:1 for large text)
- Forms: visible labels, error messages associated via `aria-describedby`
- Modals/dialogs: focus trap, `aria-modal`, close on Escape
- Loading states: `aria-busy`, visible spinner + text label
- Icons used alone (no visible label): must have `aria-label`

---

## 13. Do's and Don'ts

### Do
- Use the purple gradient for hero sections and primary CTAs
- Maintain generous whitespace between sections
- Use card-based layouts with consistent shadows
- Use pill-shaped tags for categories
- Show loading spinners during async operations
- Show empty states with helpful messages and CTAs

### Don't
- Use pure black (`#000000`) for text — use `#111827` (near-black)
- Place text over images without a gradient overlay
- Mix border-radius styles within the same component group
- Use more than 2 accent colors alongside the purple palette
- Override U-ERRE purple with arbitrary brand colors
