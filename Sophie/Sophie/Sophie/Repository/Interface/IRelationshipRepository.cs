using System;
using System.Collections.Generic;
using Sophie.Resource.Entities;

namespace Sophie.Repository.Interface
{
    public interface IRelationshipRepository
    {
        Relationship CreateRelationship(Relationship item);
        Relationship RestoreRelationship(Relationship item);
        Relationship DeleteRelationship(string relationshipId);
        List<Relationship> ListRelationship(int pageIndex = 0, int pageSize = 99);
        Relationship UpdateRelationship(Relationship item);
        long TotalRelationship();

        Relationship FindByIdRelationship(string relationshipId);
        List<Relationship> FindByIdHospital(string hospitalId);
        List<Relationship> FindByIdDoctor(string doctorId);
    }
}
