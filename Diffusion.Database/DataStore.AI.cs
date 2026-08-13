using System;
using System.Collections.Generic;
using System.Linq;
using Diffusion.Database.Models;

namespace Diffusion.Database;

public partial class DataStore
{
    /// <summary>
    /// Upsert the AI analysis for an image (keyed by ImageId, 1:1).
    /// </summary>
    public void SaveAiAnalysis(AiImageAnalysis analysis)
    {
        using var db = OpenConnection();
        var existing = db.Table<AiImageAnalysis>().FirstOrDefault(a => a.ImageId == analysis.ImageId);
        var now = DateTime.Now;
        if (existing == null)
        {
            analysis.CreatedDate = now;
            analysis.UpdatedDate = now;
            db.Insert(analysis);
        }
        else
        {
            analysis.Id = existing.Id;
            analysis.CreatedDate = existing.CreatedDate;
            analysis.UpdatedDate = now;
            db.Update(analysis);
        }
    }

    /// <summary>
    /// Return the saved AI analysis for an image, or null.
    /// </summary>
    public AiImageAnalysis? GetAiAnalysis(int imageId)
    {
        using var db = OpenConnection();
        return db.Table<AiImageAnalysis>().FirstOrDefault(a => a.ImageId == imageId);
    }

    /// <summary>
    /// Given a set of image ids, return those that have no saved AI analysis yet.
    /// Used by the future batch-analyze feature.
    /// </summary>
    public List<int> GetUnanalyzedImages(IEnumerable<int> imageIds)
    {
        var ids = imageIds as List<int> ?? imageIds.ToList();
        if (ids.Count == 0)
            return new List<int>();

        using var db = OpenConnection();
        var analyzed = new HashSet<int>(
            db.Table<AiImageAnalysis>().Where(a => ids.Contains(a.ImageId)).Select(a => a.ImageId)
        );
        return ids.Where(id => !analyzed.Contains(id)).ToList();
    }

    /// <summary>
    /// Delete the saved AI analysis for an image.
    /// </summary>
    public void DeleteAiAnalysis(int imageId)
    {
        using var db = OpenConnection();
        var existing = db.Table<AiImageAnalysis>().FirstOrDefault(a => a.ImageId == imageId);
        if (existing != null)
        {
            db.Delete(existing);
        }
    }
}
