﻿using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NitroxModel.DataStructures;
using NitroxModel.DataStructures.GameLogic;
using ProtoBufNet;

namespace NitroxServer.GameLogic.Entities
{
    [ProtoContract]
    public class EntityData
    {
        [ProtoMember(1)]
        public List<Entity> Entities = new List<Entity>();

        [ProtoAfterDeserialization]
        private void ProtoAfterDeserialization()
        {
            // After deserialization, we want to assign all of the 
            // children to their respective parent entities.
            Dictionary<NitroxId, Entity> entitiesById = Entities.ToDictionary(entity => entity.Id);

            foreach (Entity entity in Entities)
            {
                if (entity.ParentId != null)
                {
                    Entity parent = entitiesById[entity.ParentId];

                    if (parent != null)
                    {
                        parent.ChildEntities.Add(entity);
                        entity.Transform.SetParent(parent.Transform);
                    }
                }
            }
        }

        [OnDeserialized]
        private void JsonAfterDeserialization(StreamingContext context)
        {
            ProtoAfterDeserialization();
        }


        public static EntityData From(List<Entity> entities)
        {
            return new EntityData { Entities = entities };
        }
    }
}
