using AutoMapper;
using FPTAlumniConnect.API.Services.Interfaces;
using FPTAlumniConnect.DataTier.Models;
using FPTAlumniConnect.DataTier.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using FPTAlumniConnect.BusinessTier.Payload;

namespace FPTAlumniConnect.API.Services.Implements
{
    public class PhoBertService : BaseService<PhoBertService>, IPhoBertService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiToken= "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";

        public PhoBertService(
            IUnitOfWork<AlumniConnectContext> unitOfWork,
            ILogger<PhoBertService> logger,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
            : base(unitOfWork, logger, mapper, httpContextAccessor)
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiToken);
        }

        public async Task<double[]> GenerateEmbedding(EmbeddingRequest text)
        {
            var requestBody = new { inputs = text }; 
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            try
            {
                // Sử dụng đúng endpoint của mô hình
                var response = await _httpClient.PostAsync("https://api-inference.huggingface.co/models/sentence-transformers/all-MiniLM-L6-v2", content);
                response.EnsureSuccessStatusCode(); // Kiểm tra thành công của response

                // Đọc và xử lý phản hồi từ API
                var responseBody = await response.Content.ReadAsStringAsync();
                var embedding = JsonSerializer.Deserialize<double[]>(responseBody);

                return embedding ?? throw new Exception("Failed to generate embedding.");
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ nếu có
                throw new Exception($"Error generating embedding: {ex.Message}");
            }
        }


        public async Task<int?> FindBestMatchingCV(EmbeddingRequest jobDescription)
        {
            var jobEmbedding = await GenerateEmbedding(jobDescription);

            var cvRepository = _unitOfWork.GetRepository<Cv>();
            var cvs = await cvRepository.GetAllAsync();

            int? bestCvId = null;
            double bestScore = -1.0;

            foreach (var cv in cvs)
            {
                if (cv.Embedding != null)
                {
                    var cvEmbedding = JsonSerializer.Deserialize<double[]>(cv.Embedding);
                    if (cvEmbedding != null)
                    {
                        var similarityScore = CalculateCosineSimilarity(jobEmbedding, cvEmbedding);
                        if (similarityScore > bestScore)
                        {
                            bestScore = similarityScore;
                            bestCvId = cv.Id;
                        }
                    }
                }
            }

            return bestCvId;
        }

        private double CalculateCosineSimilarity(double[] vecA, double[] vecB)
        {
            double dotProduct = 0.0, magnitudeA = 0.0, magnitudeB = 0.0;
            for (int i = 0; i < vecA.Length; i++)
            {
                dotProduct += vecA[i] * vecB[i];
                magnitudeA += Math.Pow(vecA[i], 2);
                magnitudeB += Math.Pow(vecB[i], 2);
            }

            magnitudeA = Math.Sqrt(magnitudeA);
            magnitudeB = Math.Sqrt(magnitudeB);

            return dotProduct / (magnitudeA * magnitudeB);
        }
    }
}
