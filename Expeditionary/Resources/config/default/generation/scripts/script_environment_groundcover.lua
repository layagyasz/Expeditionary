luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')

Bare = MapEnvironmentModifier()
Bare.Key = "environment-modifier-groundcover-bare"
Bare.Name = "Bare"
function Bare:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0
end

Sparse = MapEnvironmentModifier()
Sparse.Key = "environment-modifier-groundcover-sparse"
Sparse.Name = "Sparse"
function Sparse:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.25
end

Balanced = MapEnvironmentModifier()
Balanced.Key = "environment-modifier-groundcover-balanced"
Balanced.Name = "Balanced"
function Balanced:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.5
end

Blanket = MapEnvironmentModifier()
Blanket.Key = "environment-modifier-groundcover-blanket"
Blanket.Name = "Blanket"
function Blanket:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.9
end

function Load()
	return { Bare, Sparse, Balanced, Blanket }
end