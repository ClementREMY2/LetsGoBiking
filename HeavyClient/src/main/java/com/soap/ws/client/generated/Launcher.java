package com.soap.ws.client.generated;
import java.nio.charset.StandardCharsets;
import java.util.List;
import java.util.Scanner;
import org.apache.activemq.ActiveMQConnectionFactory;
import javax.jms.*;

public class Launcher {

    public static void main(String[] args) throws JMSException {
        System.out.println("Hello from the heavy client!");
        RSService service = new RSService();
        RSIService port = service.getBasicHttpBindingRSIService();
        while(true) {
            Scanner s1 = new Scanner(System.in);
            System.out.println("press x at anytime to exit");
            System.out.println("Enter the departure point");
            String departure = s1.nextLine();
            if (departure.equals("x"))
            {
                break;
            }
            System.out.println("Enter the arrival point");
            String arrival = s1.nextLine();
            if (arrival.equals("x"))
            {
                break;
            }

            Itineraries itineraries = port.getItinerary(departure,arrival);
            //If there is no itinerary in the response, it means that activeMQ works and we'll open a consumer.
            if (itineraries.getItineraries().getValue().getItinerary().size() == 0)
            {

                String brokerURL = "tcp://localhost:61616";
                ConnectionFactory connectionFactory = new ActiveMQConnectionFactory(brokerURL);
                Connection connection = connectionFactory.createConnection();
                connection.start();

                try {
                    Session session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
                    Destination destination = session.createQueue("queueName");

                    MessageConsumer consumer = session.createConsumer(destination);

                    consumer.setMessageListener(new MessageListener() {
                        @Override
                        public void onMessage(Message message) {
                            try {
                                // Spécifie l'encodage UTF-8 lors de la conversion du message en texte (ne marche pas)
                                String textMessage = ((TextMessage) message).getText();

                                //Affichage
                                System.out.println("Message reçu depuis ActiveMQ : " + new String(textMessage.getBytes(StandardCharsets.UTF_8)));
                            } catch (JMSException e) {
                                e.printStackTrace();
                            }

                        }

                    });
                } catch (JMSException e) {
                    e.printStackTrace();
                }

            }
            else {
                List<Itinerary> listOfItineraries = itineraries.getItineraries().getValue().getItinerary();
                for (Itinerary itinerary : listOfItineraries) {
                    for (Segment segment : itinerary.getSegments().getValue().getSegment()) {
                        for (Step step : segment.getSteps().getValue().getStep()) {
                            System.out.println(step.getInstruction().getValue());
                        }
                    }
                }
                System.out.println("");
                for (int i = 0; i < listOfItineraries.size(); i++) {
                    var root = listOfItineraries.get(i);
                    var duration = root.getSegments().getValue().getSegment().get(0).getDuration();
                    int days = (int) Math.floor(duration/86400);
                    int hours = (int) Math.floor(duration/3600);
                    int minutes = (int) Math.floor((duration % 3600) / 60);
                    double seconds = duration % 60;
                    String wayToGo = "";
                    if (i % 2 == 0) {
                        wayToGo = "Walk: ";
                    }
                    else {
                        wayToGo = "Bike: ";
                    }

                    if (duration > 60) {

                        if (duration > 3600) {
                            if (duration > 86400) {
                                if (hours > 24) {
                                    hours = hours - 24*days;
                                }
                                System.out.println(wayToGo + days + " d, " + hours + " h, " + minutes + " min and " + seconds + " s");
                            }
                            else {
                                System.out.println(wayToGo + hours + " h, " + minutes + " min and " + seconds + " s");
                            }
                        } else {
                            System.out.println(wayToGo + minutes + " min and " + seconds + " s");
                        }
                    }
                }
            }
            try {
                Thread.sleep(1000);
            } catch (InterruptedException e) {
                throw new RuntimeException(e);
            }
        }
        System.out.println("Goodbye from the heavy client!");
        System.exit(0);
    }
}
