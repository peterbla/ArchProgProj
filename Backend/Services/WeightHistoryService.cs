using System.Security.Cryptography.X509Certificates;

using Backend.DatabaseModels;

using Models;

using Supabase;
using Supabase.Postgrest.Responses;

namespace Backend.Services
{
    public class WeightHistoryService(Client supabaseClient)
    {
        private readonly Client _supabaseClient = supabaseClient;

        public async Task<List<ReturnedWeightHistory>> GetUserWeightHistory(User user)
        {
            ModeledResponse<WeightHistory> weightHistory = await _supabaseClient.From<WeightHistory>()
                .Where(x => x.UserId == user.Id)
                .Order(
                    "datetime",
                    Supabase.Postgrest.Constants.Ordering.Descending,
                    Supabase.Postgrest.Constants.NullPosition.Last
                )
                .Get();
            return weightHistory.Models.Select(x => x.ConwertToReturn()).ToList();
        }

        public async Task<ReturnedWeightHistory> GetUserSingleWeight(User user, int id)
        {
            WeightHistory? weightHistory = await _supabaseClient.From<WeightHistory>()
                .Where(x => x.UserId == user.Id && x.Id == id)
                .Single();
            if (weightHistory == null)
            {
                throw new Exception("Weight doesn't exist.");
            }

            ReturnedWeightHistory returnedWeightHistory = weightHistory.ConwertToReturn();

            return returnedWeightHistory;
        }

        public async Task<ReturnedWeightHistory> GetUserNewestWeight(User user)
        {
            List<ReturnedWeightHistory> weightHistory = await GetUserWeightHistory(user);
            if (!weightHistory.Any())
            {
                throw new Exception("Weight history is empty");
            }
            return weightHistory[0];
        }

        public async Task<ReturnedWeightHistory> AddNewUserWeight(User user, NewWeightHistory newWeightHistory)
        {
            WeightHistory weightHistory = new()
            {
                UserId = user.Id,
                Datetime = newWeightHistory.Datetime,
                WeightKg = newWeightHistory.WeightKg,
            };

            ModeledResponse<WeightHistory> databaseWeightHistory = await _supabaseClient.From<WeightHistory>().Insert(weightHistory);

            ReturnedWeightHistory returnedWeightHistory = databaseWeightHistory.Model?.ConwertToReturn() ?? weightHistory.ConwertToReturn();
            ReturnedWeightHistory userNewestWeightHistory = await GetUserNewestWeight(user);

            user.WeightKg = userNewestWeightHistory.WeightKg;

            await user.Update<User>();

            return returnedWeightHistory;
        }
    }
}
