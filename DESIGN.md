# Student Groups Hub Design System

## 1. Atmosphere & Identity

An institutional student-community hub that feels welcoming, energetic, and trustworthy. Its signature is the U-ERRE purple gradient paired with compact white operational surfaces and amber event accents; responsive work preserves that identity rather than introducing a new visual direction.

## 2. Color

| Role | Token | Value | Usage |
|---|---|---|---|
| Brand primary | `--uerre-purple` | `#5b21b6` | Primary actions and focus |
| Brand dark | `--uerre-purple-dark` | `#4c1d95` | Gradient depth and hover |
| Brand light | `--uerre-purple-light` | `#7c3aed` | Gradient and focus |
| Brand vivid | `--uerre-purple-vivid` | `#8b5cf6` | Highlights |
| Accent | `--uerre-gold` | `#f59e0b` | Events and pending attention |
| Surface | `--uerre-surface` | `#f8f7ff` | Page background |
| Surface alternate | `--uerre-surface-alt` | `#f3f4f6` | Secondary regions |
| Text | `--uerre-text` | `#374151` | Body content |
| Heading | `--uerre-heading` | `#111827` | Headings |
| Muted | `--uerre-muted` | `#9ca3af` | Secondary metadata |
| Border | `--uerre-border` | `#e5e7eb` | Surface separation |

Status colors retain the existing Bootstrap-compatible success, warning, danger, and neutral ramps. New UI must use these tokens or Bootstrap semantic variables rather than introducing another accent family.

## 3. Typography

- Heading family: Montserrat, 600-800.
- Body family: Open Sans, 400-600.
- Page title: `clamp(1.5rem, 4vw, 2.25rem)`.
- Section title: `1.25rem` to `1.5rem`.
- Body: `1rem`; compact operational text: `.875rem`; metadata: `.75rem`.
- KPI values use tabular figures, Montserrat 800, and never shrink below `1.75rem`.

## 4. Spacing & Layout

- Base spacing unit: `4px`.
- Content maximum: `1200px`.
- Standard inline page gutter: `24px`, reduced to `16px` at 768px and `12px` at 390px.
- Standard card padding: `24px`, reduced to `16px` on narrow screens.
- Shared responsive layout primitives: wrapping action cluster, intrinsic KPI grid, vertical mobile stack, and bounded modal body.
- Primary content must reflow into one readable column at 375px with no document-level horizontal scrollbar.

## 5. Components

### Card
- **Structure**: semantic content inside `.card-uerre` or `.stat-card`.
- **States**: default, hover where interactive, loading skeleton, empty, and error.
- **Layout**: content may wrap; every flex child containing text uses `min-width: 0`.

### Action cluster
- **Structure**: related buttons or links in a wrapping flex container.
- **States**: default, hover, active, focus-visible, disabled, busy.
- **Layout**: full-width stacked controls at 375px; intrinsic width from tablet upward.

### Responsive modal
- **Structure**: labelled dialog, header, scrollable body, footer actions.
- **States**: default, validation error, saving, success.
- **Layout**: near-full viewport sheet on mobile; centered bounded dialog on desktop.

### KPI card
- **Structure**: label, primary value, contextual comparison or definition.
- **States**: loading skeleton, zero-data explanation, positive/negative/neutral delta.
- **Layout**: intrinsic grid with one column on the narrowest screens, two on mobile, three on tablet, six where space permits.

### Calendar
- **Desktop**: seven-column month grid with selectable event days.
- **Mobile**: date-grouped agenda cards; month navigation remains visible and does not overflow.

## 6. Motion & Interaction

- Micro interactions: 150-200ms ease-out.
- Surface transitions: 200-300ms ease-in-out.
- Animate only transform, opacity, color, border color, and shadow.
- Preserve visible focus and respect `prefers-reduced-motion`.

## 7. Depth & Surface

Use the existing mixed strategy: subtle cool borders plus low-opacity purple-tinted shadows. Elevated modals may use the existing prominent shadow; ordinary operational cards stay restrained.

## 8. Accessibility Constraints & Accepted Debt

- Target WCAG 2.2 AA with visible keyboard focus, labelled controls, and 44px mobile touch targets where space permits.
- Mobile reflow must preserve reading order and not rely on horizontal scrolling for primary content.
- Existing hand-built modal focus trapping remains accepted debt outside the three approved priorities; new event editing must at least expose a labelled dialog and keyboard-close control.
