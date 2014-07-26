<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="2.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

  <xsl:template match="/">
    <div id="tableContainer">
      <table  align="left" bgcolor="white" cellspacing="1" cellpadding="0"  class="inner_tb"  border="1" isHeader="1" id="tblBrands" consider="1" startRow="1" parameter="DSARHeader">
        <!-- New Code -->
        <tr class="tableheaderFloat" >
          <th> </th>
          <th align="center" class="tableheader" >Total Visits --></th>
          <th> </th>
          <!-- Code to  Total Visit Starts -->
          <xsl:choose>
            <xsl:when test="count(//row[@Element='TotalVisits']) &gt; 0" >
              <xsl:for-each select="//row[@Element='TotalVisits']">
                <th width="25px">
                  <xsl:value-of select="@Frequency" />
                </th>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
          <!-- Code to  Total Visit Ends -->
          <th align="center" colspan="8">
            Status - <xsl:value-of select="//row[@Element='Status']/@SaveFlag" />
          </th>
        </tr>

        <!-- End -->

        <tr class="tableheaderFloat" align="center" >
          <th width="5px" class="tableheader" >Focus Doctor</th>
          <th class="tableheader"  >Doctor</th>
          <th class="tableheader" >Specialty</th>
          <!-- Code to  Total Visit Starts -->
          <xsl:choose>
            <xsl:when test="count(//row[@Element='CycleMonth']) &gt; 0" >
              <xsl:for-each select="//row[@Element='CycleMonth']">
                <th width="25px">
                  <xsl:value-of select="@cyclesMonth" />-<xsl:value-of select="@cyclesYear" />
                </th>
              </xsl:for-each>
            </xsl:when>
          </xsl:choose>
          <!-- Code to  Total Visit Ends -->
          <th align="center" width ="17%">Products</th>
          <th align="center" width ="2%">D/R</th>
          <th align="center" width ="10%">Driver</th>
          <th align="center" width ="6%">PL</th>
          <th align="center" width ="10%">Class</th>
          <th align="center" width ="7%">Cap.</th>
          <th align="center" width ="15%">Share%</th>
          <th align="center" width ="25%">Cycle Object</th>
        </tr>

        <xsl:choose>
          <xsl:when test="count(//row[@Element='DocDetails']/@DoctorFKID) &gt;0" >
            <xsl:apply-templates select="//row[@Element='DocDetails']"/>
          </xsl:when>
          <xsl:otherwise>
            <tr class="tablecontent1">
              <td align="center" class="alerttext" colspan="17" >No Records Found !</td>
            </tr>
          </xsl:otherwise>
        </xsl:choose>
      </table>
    </div>
  </xsl:template>

  <xsl:template match="//row[@Element='DocDetails']">

    <xsl:variable name="plan-mon" select="//row[@Element='CycleMonth']/@PMonth" />

    <xsl:variable name="plan-yr" select="//row[@Element='CycleMonth']/@PYear" />

    <xsl:variable name="Dr-id" select="@DoctorFKID" />

    <xsl:variable name="Dr-id-child" select="//row[@Element='FreqDetail']/@DoctorFKID" />

    <tr>
      <td class="tb-r2">
        <xsl:attribute name="nowrap"/>
        <xsl:value-of select="@FocusFlag" />
      </td>
      <td class="tb-r2">
        <xsl:attribute name="nowrap"/>
        <xsl:value-of select="@DoctorName" />&#160;<xsl:value-of select="@MiddleName" />&#160;<xsl:value-of select="@LastName" />
      </td>
      <td class="tb-r2">
        <xsl:attribute name="nowrap"/>
        <xsl:value-of select="@SPecialization" />
      </td>

      <xsl:for-each select ="//row[@Element='FreqDetail' and @DoctorFKID=$Dr-id and @PlanMonth=$plan-mon and @PlanYear=$plan-yr]">
        <td class="tb-r2">
          <xsl:value-of select="@Frequency" />
        </td>
      </xsl:for-each>

      <td class="tb-r2">
        <!--<table bgcolor="white" cellspacing="1" cellpadding="0"  class = "tablebg"> -->
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <!--<tr class="tablecontent2">
				<td> -->
          <xsl:value-of select="@ProductName"></xsl:value-of>
          <br/>
          <!--</td>
					</tr> -->
        </xsl:for-each>

        <!--</table> -->
      </td>
      <td class="tb-r2">
        <!--<table bgcolor="white" cellspacing="1" cellpadding="0"  class = "tablebg"> -->
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <!--<tr class="tablecontent2">
				<td> -->
          <xsl:value-of select="@GenName"></xsl:value-of>
          <br/>
          <!--</td>
					</tr> -->
        </xsl:for-each>

        <!--</table> -->
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@Drivername"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@PLStage"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@ClassName"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@QualifiedDrCapacity"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@QualifiedDrShare"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
      <td class="tb-r2">
        <xsl:for-each select ="//row[@Element='Prodetail' and @DoctorFKID=$Dr-id]">
          <xsl:attribute name="nowrap"/>
          <xsl:value-of select="@CycleObjective"></xsl:value-of>
          <br/>
        </xsl:for-each>
      </td>
    </tr>
  </xsl:template>

</xsl:stylesheet>