using System;
using System.Linq;
using UGCS.Sdk.Protocol.Encoding;

namespace SlideMap.Services
{
    public class MappingRequestService
    {
        protected GetMappingResponse _responseAlghoritms;

        public MappingRequestService()
        {
            _requestAlgorithms();
        }

        private void _requestAlgorithms()
        {
            if (SlideMapModule.Executor != null)
            {
                var request = new GetMappingRequest()
                {
                    ClientId = SlideMapModule.ClientId,
                    GetAlgorithms = true
                };
                var response = SlideMapModule.Executor.Submit<GetMappingResponse>(request);
                response.Wait();
                _responseAlghoritms = response.Value;
            }
        }


        /// <summary>
        /// Returns avaliable waypoint algorithm object by class name
        /// </summary>
        /// <param name="algorithmClassName">algorithms lass name</param>
        /// <returns></returns>
        public TraverseAlgorithm GetAlgoritmByClassName(String algorithmClassName)
        {
            foreach (AlgorithmsDto algorithmsDto in _responseAlghoritms.AlgorithmsMapping.Algorithms)
            {
                TraverseAlgorithm algorithm = algorithmsDto.AlgorithmAndActionCodes
                    .Where(id => id.Algorithm.GetClassName() == algorithmClassName)
                    .Select(x => x.Algorithm)
                    .FirstOrDefault<TraverseAlgorithm>();
                if (algorithm != null)
                    return algorithm;
            }
            return null;
        }

    }
}
