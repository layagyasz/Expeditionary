luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Bare = MapEnvironmentTrait()
Bare.Key = "environment-trait-groundcover-bare"
Bare.Name = "Bare"
function Bare:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0
end

Sparse = MapEnvironmentTrait()
Sparse.Key = "environment-trait-groundcover-sparse"
Sparse.Name = "Sparse"
function Sparse:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.25
end

Balanced = MapEnvironmentTrait()
Balanced.Key = "environment-trait-groundcover-balanced"
Balanced.Name = "Balanced"
function Balanced:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.5
end

Blanket = MapEnvironmentTrait()
Blanket.Key = "environment-trait-groundcover-blanket"
Blanket.Name = "Blanket"
function Blanket:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.9
end

function Load()
	return { Bare, Sparse, Balanced, Blanket }
end