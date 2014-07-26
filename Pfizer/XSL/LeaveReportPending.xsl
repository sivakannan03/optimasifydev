<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <div id="Div_container">
      <table id="tblLeavePending" align="left" cellspacing="1" cellpadding="0"  class="inner_tb"  border="1"  width="100%">
        <thead>
          <tr  class="tableheader">
            <th colspan="11">Pending</th>
          </tr>
          <tr class="tableheader">
            <th align="center" width="4px">S.No</th>
            <th align="center">Code</th>
            <th align="center" width="165px">Name</th>
            <th align="center" width="100px">Leave Type</th>
            <th align="center" width="40px">Availed</th>
            <th align="center" width="40px">Balance</th>
            <th width="65px">On process</th>
            <th align="center" width="70px">From Date</th>
            <th align="center" width="70px">To Date</th>
            <th width="95px">Requested Date</th>
            <th align="center" width="100px">Reason</th>
          </tr>
        </thead>
        <xsl:choose>
          <xsl:when test="count(//row[@Element='SUBMITTED']) &gt; 0">
            <xsl:for-each select="//row[@Element='SUBMITTED']">
              <xsl:variable name="sGetNodePKID" select="@NodePKID" />
              <tr>
                <xsl:if test="(position() mod 2)=0">
                  <xsl:attribute name="class" >tableContent1</xsl:attribute>
                </xsl:if>
                <xsl:if test="(position() mod 2)!=0">
                  <xsl:attribute name="class" >tableContent2</xsl:attribute>
                </xsl:if>
                <td align="right">
                  <xsl:value-of select="position()" />
                </td>
                <td >
                  <xsl:value-of select="@EmpCode"/>
                </td>
                <td>
                  <xsl:value-of select="@EmployeeName"/>
                </td>
                <td>
                  <xsl:value-of select="@LeaveGenName"/>
                </td>
                <td>
                  <xsl:value-of select="@Leaveavailed"/>
                </td>
                <td>
                  <xsl:value-of select="@LeaveBalance"/>
                </td>
                <td>
                  <xsl:value-of select="@OnProcess"/>
                </td>
                <td>
                  <xsl:value-of select="@Fromdate"/>
                </td>
                <td>
                  <xsl:value-of select="@Todate"/>
                </td>
                <td>
                  <xsl:value-of select="@CreatedDate"/>
                </td>
                <td>
                  <xsl:value-of select="@Reason"/>
                </td>
              </tr>
            </xsl:for-each>
          </xsl:when>
          <xsl:otherwise>
            <tr class="tablecontent1">
              <td align="center" class="alerttext" colspan="11" >No Records Found !</td>
            </tr>
          </xsl:otherwise>
        </xsl:choose>
      </table>
    </div>
  </xsl:template>
</xsl:stylesheet>