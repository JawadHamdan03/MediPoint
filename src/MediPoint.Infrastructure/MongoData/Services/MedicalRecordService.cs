using MediPoint.Application.Common;
using MediPoint.Domain.Entities.MedicalRecords;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.MongoData.Services;

public class MedicalRecordService(IOptions<MongoDbContext> mongoDbContext):IMedicalRecordsService
{
    private readonly IMongoCollection<MedicalRecord> _MedicalRecordsCollection = new MongoClient(mongoDbContext.Value.ConnectionString)
       .GetDatabase(mongoDbContext.Value.DatabaseName)
       .GetCollection<MedicalRecord>(mongoDbContext.Value.MedicalRecordsCollectionName);

    public async Task<List<MedicalRecord>> GetAsync() =>
       await _MedicalRecordsCollection.Find(_ => true).ToListAsync();

    public async Task<MedicalRecord?> GetAsync(string id) =>
        await _MedicalRecordsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(MedicalRecord medRecord) =>
        await _MedicalRecordsCollection.InsertOneAsync(medRecord);

    public async Task UpdateAsync(string id, MedicalRecord updatedMedRecord) =>
        await _MedicalRecordsCollection.ReplaceOneAsync(x => x.Id == id, updatedMedRecord);

    public async Task RemoveAsync(string id) =>
        await _MedicalRecordsCollection.DeleteOneAsync(x => x.Id == id);

}
