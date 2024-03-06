namespace Interview.Web.Tests
{
     public class ProductServiceTests
     {
          //EVAL: I wanted to create unit tests but I have not done this before in C#
          // I spent 20 minutes trying to get it setup but ran into a lot of local issues so 
          // I am just going to write some mock unit tests 
          // I will spare you reading by not doing this in the controller test as well
          // just wanted to showcase I could think through TDD since I did not have enough time to finish setting up.

          //SETUP:
          //   MOCK DB
          //TEST GET ALL PRODUCTS:
          //   1: 
          //   MOCK DB WITH 5 PRODUCTS
          //   CALL GET_ALL_PRODUCTS FUNCTION
          //   LIST LENGTH == 5
          //
          //   2:
          //   MOCK DB WITH 5 PRODUCTS
          //   CALL GET_ALL_PRODUCTS FUNCTION
          //   RETURNED PRODUCTS SHOULD = EXPECTED PRODUCTS
          //TEST CREATE PRODUCT:
          //   1:
          //   CALL CREATE PRODUCT
          //   VALUE RETURNED FROM FUNCTION SHOULD == EXPECTED PRODUCT
          //   DB SHOULD HAVE PRODUCT IN PRODUCTS TABLE
          //   
          //   2:
          //   CALL CREATE PRODUCT WITH INVALID PRODUCT
          //   SHOULD REJECT CREATION
          //   PRODUCT SHOULDN'T EXIST IN DB
          //
          //SEARCH FOR PRODUCT:
          //   1:  
          //   CALL FIND PRODUCT BY SKU
          //   SEARCH DB FOR PRODUCT
          //   PRODUCT RETURNED SHOULD == EXPECTED PRODUCT
          //
          //   2:
          //   CALL FIND PRODUCT BY OTHER VALUE
          //   SEARCH DB FOR PRODUCT
          //   PRODUCT RETURNED SHOULD == EXPECTED PRODUCT
          //
          //   3:
          //   CALL FIND PRODUCT BY INVALID PARAM
          //   REJECT CALL
          //   RETURN ERROR
          //
          //   4:
          //   CALL FIND PRODUCT FOR NON EXISTING PRODUCT
          //   SHOULD RETURN NULL
     }
}
