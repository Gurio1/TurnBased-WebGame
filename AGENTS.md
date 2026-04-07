## Documentation maintenance rules

- Every code or behavior change must be logged in `AGENTS.md` under `## What was implemented in this session`.
- After any code or behavior change, always build the affected application(s) before ending the task and report the result.

## Project structure

- `Game/` - main ASP.NET Core game service, including monster admin contracts, application services, controllers, and loot generation.
- `Game.Battle/` - battle service and reward transport models used after combat settlement.
- `Game.SharedKernel/` - shared battle snapshot and reward DTOs used between services.
- `ClientApp/src/features/admin/blueprints/` - equipment blueprint admin workshop.
- `ClientApp/src/features/admin/monsters/` - monster admin workshop with drag-and-drop stats, abilities, and mixed-item drops.