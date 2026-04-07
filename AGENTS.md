## Documentation maintenance rules

- Every code or behavior change must be logged in `AGENTS.md` under `## What was implemented in this session`.
- After any code or behavior change, always build the affected application(s) before ending the task and report the result.

## What was implemented in this session

- Fixed home-page inventory item actions causing browser CORS failures by generating relative `players/equip` and `players/sell` paths instead of absolute backend URLs, and by moving HTTPS redirection ahead of CORS so preflight requests are not redirected before the CORS policy runs.
