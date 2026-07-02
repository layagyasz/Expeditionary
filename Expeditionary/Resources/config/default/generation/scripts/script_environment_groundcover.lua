luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

MapEnvironmentTrait=luanet.import_type('Expeditionary.Model.Mapping.Environments.MapEnvironmentTrait')

Bare = MapEnvironmentTrait()
Bare.Key = "environment-trait-groundcover-bare"
function Bare:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0
end

Sparse = MapEnvironmentTrait()
Sparse.Key = "environment-trait-groundcover-sparse"
function Sparse:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.25
end

Balanced = MapEnvironmentTrait()
Balanced.Key = "environment-trait-groundcover-balanced"
function Balanced:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.5
end

Blanket = MapEnvironmentTrait()
Blanket.Key = "environment-trait-groundcover-blanket"
function Blanket:Apply(environment)
	environment.Parameters.Terrain.GroundCoverCover = 0.9
end

function Load()
	return { Bare, Sparse, Balanced, Blanket }
end