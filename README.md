# Team Ranking API
## Overview
The Team Ranking API is a RESTful web service to manage teams, matches, and rankings in a sports league. It provides endpoints for CRUD operations on teams and matches, and automatically updates team rankings based on match results.

## Features
Manage teams (create, read, update, delete)\
Manage matches (create, read, update, delete)\
Automatic ranking updates based on match results\
Points system: Win = 3 points, Draw = 1 point, Loss = 0 points\
Swagger for API documentation

## API Endpoints
### Teams
GET /api/teams: Retrieve all teams.\
GET /api/teams/{id}: Retrieve a specific team by ID.\
POST /api/teams: Create a new team.\
PUT /api/teams/{id}: Update an existing team.\
DELETE /api/teams/{id}: Delete a team.
### Bulk Import
POST /api/teams/import-teams-from-file: Import teams from a JSON file.
### Matches
GET /api/matches: Retrieve all matches.\
GET /api/matches/{id}: Retrieve a specific match by ID.\
POST /api/matches: Create a new match.\
PUT /api/matches/{id}: Update an existing match.\
DELETE /api/matches/{id}: Delete a match.
### Rankings
GET /api/rankings: Retrieve all team rankings.\
GET /api/rankings/{teamId}: Retrieve the ranking of a specific team.
