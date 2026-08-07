using MediPoint.Application.Common;
using MediPoint.Domain.Entities.Prescriptions.LabRes;
using MediPoint.Domain.Entities.Prescriptions.Med;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.MongoData.Services;

public class LabResultService(IOptions<MongoDbContext> mongoDbContext):ILabResultService
{
    private readonly IMongoCollection<LabResult> _labResultsCollection = new MongoClient(mongoDbContext.Value.ConnectionString)
       .GetDatabase(mongoDbContext.Value.DatabaseName)
       .GetCollection<LabResult>(mongoDbContext.Value.LabResultsCollectionName);


    public async Task<List<LabResult>> GetAsync() =>
      await _labResultsCollection.Find(_ => true).ToListAsync();

    public async Task<LabResult?> GetAsync(string id) =>
        await _labResultsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(LabResult medRecord) =>
        await _labResultsCollection.InsertOneAsync(medRecord);

    public async Task UpdateAsync(string id, LabResult updatedMedRecord) =>
        await _labResultsCollection.ReplaceOneAsync(x => x.Id == id, updatedMedRecord);

    public async Task RemoveAsync(string id) =>
        await _labResultsCollection.DeleteOneAsync(x => x.Id == id);
}
