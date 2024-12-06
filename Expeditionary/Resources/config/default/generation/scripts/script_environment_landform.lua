luanet.load_assembly('Cardamom', 'Cardamom.ImageProcessing.Filters')
luanet.load_assembly('Expeditionary', 'Expeditionary.Model')

Evaluator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Evaluator')
Interpolator=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Interpolator')
MapEnvironmentModifier=luanet.import_type('Expeditionary.Model.Mapping.MapEnvironmentModifier')
Treatment=luanet.import_type('Cardamom.ImageProcessing.Filters.LatticeNoise+Treatment')

CrystallineA = MapEnvironmentModifier()
CrystallineA.Key = "environment-modifier-landform-crystalline-a"
CrystallineA.Name = "Crystalline A"
function CrystallineA:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.Divergence
	terrain.ElevationNoise.Interpolator = Interpolator.HermiteSigmoid
	terrain.ElevationNoise.PreTreatment = Treatment.None
	terrain.ElevationNoise.PostTreatment = Treatment.Ridge
end

CrystallineB = MapEnvironmentModifier()
CrystallineB.Key = "environment-modifier-landform-crystalline-b"
CrystallineB.Name = "Crystalline B"
function CrystallineB:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.Curl
	terrain.ElevationNoise.Interpolator = Interpolator.HermiteQuintic
	terrain.ElevationNoise.PreTreatment = Treatment.None
	terrain.ElevationNoise.PostTreatment = Treatment.SemiRidge
end

CrystallineC = MapEnvironmentModifier()
CrystallineC.Key = "environment-modifier-landform-crystalline-c"
CrystallineC.Name = "Crystalline C"
function CrystallineC:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.ParabolicInverse
	terrain.ElevationNoise.Interpolator = Interpolator.Linear
	terrain.ElevationNoise.PreTreatment = Treatment.None
	terrain.ElevationNoise.PostTreatment = Treatment.None
end

CrystallineD = MapEnvironmentModifier()
CrystallineD.Key = "environment-modifier-landform-crystalline-d"
CrystallineD.Name = "Crystalline D"
function CrystallineD:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.VerticalEdgeInverse
	terrain.ElevationNoise.Interpolator = Interpolator.Cosine
	terrain.ElevationNoise.PreTreatment = Treatment.SemiRidge
	terrain.ElevationNoise.PostTreatment = Treatment.None
end

LithicA = MapEnvironmentModifier()
LithicA.Key = "environment-modifier-landform-lithic-a"
LithicA.Name = "Lithic A"
function LithicA:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.Gradient
	terrain.ElevationNoise.Interpolator = Interpolator.Linear
	terrain.ElevationNoise.PreTreatment = Treatment.None
	terrain.ElevationNoise.PostTreatment = Treatment.None
end

LithicB = MapEnvironmentModifier()
LithicB.Key = "environment-modifier-landform-lithic-b"
LithicB.Name = "Lithic B"
function LithicB:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.TriangularEdge
	terrain.ElevationNoise.Interpolator = Interpolator.Cosine
	terrain.ElevationNoise.PreTreatment = Treatment.None
	terrain.ElevationNoise.PostTreatment = Treatment.Billow
end

LithicC = MapEnvironmentModifier()
LithicC.Key = "environment-modifier-landform-lithic-c"
LithicC.Name = "Lithic C"
function LithicC:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.VerticalEdgeInverseDisplaced
	terrain.ElevationNoise.Interpolator = Interpolator.Hermite
	terrain.ElevationNoise.PreTreatment = Treatment.SemiRidge
	terrain.ElevationNoise.PostTreatment = Treatment.Billow
end

LithicD = MapEnvironmentModifier()
LithicD.Key = "environment-modifier-landform-lithic-d"
LithicD.Name = "Lithic D"
function LithicD:Apply(environment)
	local parameters = environment.Parameters
	local terrain = parameters.Terrain

	terrain.ElevationNoise.Evaluator = Evaluator.VerticalEdgeInverseDisplaced
	terrain.ElevationNoise.Interpolator = Interpolator.Hermite
	terrain.ElevationNoise.PreTreatment = Treatment.SemiRidge
	terrain.ElevationNoise.PostTreatment = Treatment.Billow
end

function Load()
	return { CrystallineA, CrystallineB, CrystallineC, CrystallineD, LithicA, LithicB, LithicC, LithicD }
end