using Algorithms.BlumBlumShub;
using Algorithms.Common;
using Algorithms.ElGamal;
using Algorithms.SHA256;
using Demo;
using Modelling.CustomTransformers;

var objectToByteArrayTransformer = new ObjectToByteArrayTransformer();
objectToByteArrayTransformer.TypeTransformers.Add(new ModellingTransformer());

var bbsKeyGenerator = new BlumBlumShubKeysGenerator();
var bbsEncryptionProvider = new BBSXorEncryptionProvider();
var bbsGeneratorN = bbsKeyGenerator.Generate().PublicKey.N;
var bbsGeneratorX0 = bbsKeyGenerator.GenerateSeed(bbsGeneratorN);
var bbsGenerator = new BlumBlumShubRngProvider(bbsGeneratorX0, bbsGeneratorN);

var elGamalKeyGenerator = new ElGamalKeysGenerator();
var elGamalEncryptionProvider = new ElGamalEncryptionProvider();

var passwordHasher = new SHA256PasswordHasher();

var dataFactory = new DemoDataFactory(elGamalEncryptionProvider, elGamalKeyGenerator, bbsEncryptionProvider, bbsKeyGenerator, bbsKeyGenerator, bbsGenerator, objectToByteArrayTransformer, passwordHasher);

var candidates = dataFactory.CreateCandidates();
var votersData = dataFactory.CreateVotersData();

var printer = new ModellingPrinter(dataFactory);

printer.PrintUsualVoting(candidates, votersData);
