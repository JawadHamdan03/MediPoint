using MediPoint.Application.Common;
using MediPoint.Domain.Entities.MedicalRecords;
using MediPoint.Domain.Entities.Prescriptions.Med;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace MediPoint.Infrastructure.MongoData.Services;

public class MedicineService(IOptions<MongoDbContext> mongoDbContext):IMedicineService
{
    private readonly IMongoCollection<Medicine> _medicineCollection = new MongoClient(mongoDbContext.Value.ConnectionString)
       .GetDatabase(mongoDbContext.Value.DatabaseName)
       .GetCollection<Medicine>(mongoDbContext.Value.MedicineCollectionName);


    public async Task<List<Medicine>> GetAsync() =>
      await _medicineCollection.Find(_ => true).ToListAsync();

    public async Task<Medicine?> GetAsync(string id) =>
        await _medicineCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(Medicine medRecord) =>
        await _medicineCollection.InsertOneAsync(medRecord);

    public async Task UpdateAsync(string id, Medicine updatedMedRecord) =>
        await _medicineCollection.ReplaceOneAsync(x => x.Id == id, updatedMedRecord);

    public async Task RemoveAsync(string id) =>
        await _medicineCollection.DeleteOneAsync(x => x.Id == id);


}
