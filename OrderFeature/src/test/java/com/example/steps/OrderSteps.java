package com.example.steps;

import com.example.model.*;
import com.example.service.OrderService;
import com.example.promotion.ThresholdDiscountPromotion;
import com.example.promotion.BuyOneGetOnePromotion;
import io.cucumber.datatable.DataTable;
import io.cucumber.java.en.Given;
import io.cucumber.java.en.When;
import io.cucumber.java.en.Then;
import io.cucumber.java.en.And;
import static org.junit.jupiter.api.Assertions.*;

import java.util.List;
import java.util.Map;

public class OrderSteps {

    private OrderService orderService;
    private Order currentOrder;

    @Given("no promotions are applied")
    public void no_promotions_are_applied() {
        orderService = new OrderService();
        // No promotions to add
    }

    @Given("the threshold discount promotion is configured:")
    public void the_threshold_discount_promotion_is_configured(DataTable dataTable) {
        List<Map<String, String>> rows = dataTable.asMaps(String.class, String.class);
        Map<String, String> config = rows.get(0);

        int threshold = Integer.parseInt(config.get("threshold"));
        int discount = Integer.parseInt(config.get("discount"));

        if (orderService == null) {
            orderService = new OrderService();
        }
        ThresholdDiscountPromotion promotion = new ThresholdDiscountPromotion(threshold, discount);
        orderService.addPromotion(promotion);
    }

    @Given("the buy one get one promotion for cosmetics is active")
    public void the_buy_one_get_one_promotion_for_cosmetics_is_active() {
        if (orderService == null) {
            orderService = new OrderService();
        }
        BuyOneGetOnePromotion promotion = new BuyOneGetOnePromotion("cosmetics");
        orderService.addBuyOneGetOnePromotion(promotion);
    }

    @When("a customer places an order with:")
    public void a_customer_places_an_order_with(DataTable dataTable) {
        List<Map<String, String>> rows = dataTable.asMaps(String.class, String.class);
        currentOrder = new Order();

        for (Map<String, String> row : rows) {
            String productName = row.get("productName");
            int quantity = Integer.parseInt(row.get("quantity"));
            int unitPrice = Integer.parseInt(row.get("unitPrice"));
            String category = row.get("category"); // May be null for some scenarios

            Product product;
            if (category != null) {
                product = new Product(productName, unitPrice, category);
            } else {
                product = new Product(productName, unitPrice);
            }

            OrderItem orderItem = new OrderItem(product, quantity);
            currentOrder.addItem(orderItem);
        }

        currentOrder = orderService.processOrder(currentOrder);
    }

    @Then("the order summary should be:")
    public void the_order_summary_should_be(DataTable dataTable) {
        List<Map<String, String>> rows = dataTable.asMaps(String.class, String.class);
        Map<String, String> expectedSummary = rows.get(0);

        if (expectedSummary.containsKey("totalAmount")) {
            int expectedTotal = Integer.parseInt(expectedSummary.get("totalAmount"));
            assertEquals(expectedTotal, currentOrder.getTotalAmount());
        }

        if (expectedSummary.containsKey("originalAmount")) {
            int expectedOriginal = Integer.parseInt(expectedSummary.get("originalAmount"));
            assertEquals(expectedOriginal, currentOrder.getOriginalAmount());
        }

        if (expectedSummary.containsKey("discount")) {
            int expectedDiscount = Integer.parseInt(expectedSummary.get("discount"));
            assertEquals(expectedDiscount, currentOrder.getDiscount());
        }
    }

    @And("the customer should receive:")
    public void the_customer_should_receive(DataTable dataTable) {
        List<Map<String, String>> expectedItems = dataTable.asMaps(String.class, String.class);

        for (Map<String, String> expectedItem : expectedItems) {
            String productName = expectedItem.get("productName");
            int expectedQuantity = Integer.parseInt(expectedItem.get("quantity"));

            OrderItem actualItem = currentOrder.getItemByProductName(productName);
            assertNotNull(actualItem, "Product " + productName + " should be in the order");
            assertEquals(expectedQuantity, actualItem.getFinalQuantity());
        }
    }
}
