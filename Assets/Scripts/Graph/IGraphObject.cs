using System.Numerics;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using static UnityEngine.EventSystems.EventTrigger;
namespace Assets.Scripts.Graph
{
public interface IGraphObject
{
        /// <summary>
        /// Kost energie, kost geen energie, levert energie op.
        /// Getal -1 < n< 1
        /// Energy wordt beïnvloed door Feel en Distance (factor).
        /// Deze waarde kan alleen ingevuld worden als er geen influences zijn ingesteld.
        /// De energy van dit Subject wordt berekend op basis van de energie die via influences van andere subjecten afkomt.
        /// Men moet door instellen van influences met andere Subjects bepalen of men hier energie van krijgt of dat dit energie kost.
        /// </summary>
        public float EnergyBalance { get; set; }

        /// <summary>
        /// Onverschillig, Bevlogen of Gepassioneerd
        /// Getal tussen 0 en 1?
        /// </summary>
        public float Feel { get; set; }

        /// <summary>
        /// Ver weg of Dichtbij of Kern
        /// Getal tussen 0 en 1?
        /// </summary>
        public float Distance { get; set; }
    }
}