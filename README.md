## Миграции

```
ef migrations add -s CarvedRockFitness.API -p CarvedRockFitness.API --context CarvedRockFitnessDbContext Initial
```

```
ef database update -s CarvedRockFitness.API -p CarvedRockFitness.API --context CarvedRockFitnessDbContext
```

## Токен

```
dotnet user-jwts create --project CarvedRockFitness.API --audience carvedrockfitnessapi --name 4
```


```
user-jwts create --project CarvedRockFitness.API --audience carvedrockfitnessapi --name 2
```