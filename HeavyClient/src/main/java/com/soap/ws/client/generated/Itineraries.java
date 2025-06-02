
package com.soap.ws.client.generated;

import javax.xml.bind.JAXBElement;
import javax.xml.bind.annotation.XmlAccessType;
import javax.xml.bind.annotation.XmlAccessorType;
import javax.xml.bind.annotation.XmlElementRef;
import javax.xml.bind.annotation.XmlType;


/**
 * <p>Classe Java pour Itineraries complex type.
 * 
 * <p>Le fragment de schéma suivant indique le contenu attendu figurant dans cette classe.
 * 
 * <pre>
 * &lt;complexType name="Itineraries"&gt;
 *   &lt;complexContent&gt;
 *     &lt;restriction base="{http://www.w3.org/2001/XMLSchema}anyType"&gt;
 *       &lt;sequence&gt;
 *         &lt;element name="itineraries" type="{http://schemas.datacontract.org/2004/07/RoutingServer}ArrayOfItinerary" minOccurs="0"/&gt;
 *       &lt;/sequence&gt;
 *     &lt;/restriction&gt;
 *   &lt;/complexContent&gt;
 * &lt;/complexType&gt;
 * </pre>
 * 
 * 
 */
@XmlAccessorType(XmlAccessType.FIELD)
@XmlType(name = "Itineraries", propOrder = {
    "itineraries"
})
public class Itineraries {

    @XmlElementRef(name = "itineraries", namespace = "http://schemas.datacontract.org/2004/07/RoutingServer", type = JAXBElement.class, required = false)
    protected JAXBElement<ArrayOfItinerary> itineraries;

    /**
     * Obtient la valeur de la propriété itineraries.
     * 
     * @return
     *     possible object is
     *     {@link JAXBElement }{@code <}{@link ArrayOfItinerary }{@code >}
     *     
     */
    public JAXBElement<ArrayOfItinerary> getItineraries() {
        return itineraries;
    }

    /**
     * Définit la valeur de la propriété itineraries.
     * 
     * @param value
     *     allowed object is
     *     {@link JAXBElement }{@code <}{@link ArrayOfItinerary }{@code >}
     *     
     */
    public void setItineraries(JAXBElement<ArrayOfItinerary> value) {
        this.itineraries = value;
    }

}
